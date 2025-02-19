@minLength(3)
@description('Name of the workload')
param workload string

@allowed(['prod', 'test'])
@description('Application environment')
param appEnv string

@description('Azure region for the non-global resources')
param location string = resourceGroup().location

@description('Domain name')
param domainName string

@description('Name of the shared resource group')
param sharedResourceGroup string

@description('Tags for the resources')
param tags object = {
  workload: workload
  appEnv: appEnv
}

@description('Resource ID of the Log Analytics workspace')
param logAnalyticsWorkspaceId string

@description('Application Insights connection string')
param applicationInsightsConnectionString string

@description('Name of the Azure App Configuration instance')
param appConfigurationName string

@description('Name of the container registry')
param containerRegistryName string

@description('Username to access the container registry')
param containerRegistryUsername string

@description('Name of the Azure Key Vault instance')
param keyVaultName string

@description('Repository of the API container image')
param apiImageRepository string

@description('Tag of the API container image')
param apiImageTag string

@description('Allowed HTTP origins for the API container app')
param apiAllowedOrigins array = []

@description('Flag to create a managed certificates for the container apps. Set to true on first run.')
param shouldBindManagedCertificate bool = false

var apiContainerAppName = '${workload}-${appEnv}-api-ca'

// shared managed identity
resource sharedIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31' existing = {
  name: '${workload}-shared-id'
  scope: resourceGroup(sharedResourceGroup)
}

resource appConfiguration 'Microsoft.AppConfiguration/configurationStores@2024-05-01' existing = {
  name: appConfigurationName
  scope: resourceGroup(sharedResourceGroup)
}

// container apps environment
resource appsEnvironment 'Microsoft.App/managedEnvironments@2024-03-01' = {
  name: '${workload}-${appEnv}-cae'
  location: location
  tags: tags
  properties: {
    appLogsConfiguration: {
      destination: 'azure-monitor'
    }
  }

  resource apiCertificate 'managedCertificates' = if (!shouldBindManagedCertificate) {
    name: 'api-cert'
    location: location
    tags: tags
    properties: {
      subjectName: dnsRecordsModule.outputs.apiAppFqdn
      domainControlValidation: 'CNAME'
    }
  }
}

// container apps environment diagnostic settings
resource appsEnvironmentDiagnosticSettings 'Microsoft.Insights/diagnosticSettings@2021-05-01-preview' = {
  name: 'log-analytics'
  scope: appsEnvironment
  properties: {
    workspaceId: logAnalyticsWorkspaceId
    logs: [
      {
        category: 'ContainerAppSystemLogs'
        enabled: true
      }
    ]
  }
}

module dnsRecordsModule 'dnsRecords.bicep' = {
  name: 'dnsRecords-${appEnv}-containerApps'
  scope: resourceGroup(sharedResourceGroup)
  params: {
    appEnv: appEnv
    domainName: domainName
    apiAppDefaultHostname: '${apiContainerAppName}.${appsEnvironment.properties.defaultDomain}'
    customDomainVerificationId: appsEnvironment.properties.customDomainConfiguration.customDomainVerificationId
  }
}

// api container app
resource apiContainerApp 'Microsoft.App/containerApps@2024-03-01' = {
  name: apiContainerAppName
  location: location
  tags: union(tags, { appName: 'api' })
  identity: {
    type: 'SystemAssigned,UserAssigned'
    userAssignedIdentities: {
      '${sharedIdentity.id}': {}
    }
  }
  properties: {
    environmentId: appsEnvironment.id
    configuration: {
      activeRevisionsMode: 'Single'
      maxInactiveRevisions: 3
      ingress: {
        external: true
        targetPort: 8080
        allowInsecure: false
        traffic: [
          {
            latestRevision: true
            weight: 100
          }
        ]
        customDomains: [
          {
            name: dnsRecordsModule.outputs.apiAppFqdn
            bindingType: shouldBindManagedCertificate ? 'Disabled' : 'SniEnabled'
            certificateId: shouldBindManagedCertificate ? null : appsEnvironment::apiCertificate.id
          }
        ]
        corsPolicy: {
          allowedOrigins: apiAllowedOrigins
          allowCredentials: true
          allowedHeaders: ['*']
          allowedMethods: ['GET', 'POST', 'PUT', 'DELETE', 'OPTIONS']
          exposeHeaders: ['Location']
          maxAge: 600
        }
      }
      registries: [
        {
          server: containerRegistryName
          username: containerRegistryUsername
          passwordSecretRef: 'container-registry-password'
        }
      ]
      secrets: [
        {
          name: 'container-registry-password'
          identity: sharedIdentity.id
          keyVaultUrl: 'https://${keyVaultName}${environment().suffixes.keyvaultDns}/secrets/container-registry-password'
        }
      ]
    }
    template: {
      containers: [
        {
          name: '${workload}-api'
          image: '${containerRegistryName}/${apiImageRepository}:${apiImageTag}'
          resources: {
            cpu: json('0.25')
            memory: '0.5Gi'
          }
          env: [
            {
              name: 'ASPNETCORE_ENVIRONMENT'
              value: appEnv
            }
            {
              name: 'APP_CONFIG_ENDPOINT'
              value: appConfiguration.properties.endpoint
            }
            {
              name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
              value: applicationInsightsConnectionString
            }
            {
              name: 'OTEL_SERVICE_NAME'
              value: apiContainerAppName
            }
          ]
        }
      ]
      scale: {
        minReplicas: appEnv == 'prod' ? 1 : 0
        maxReplicas: appEnv == 'prod' ? 3 : 1
        rules: [
          {
            name: 'http-scale-rule'
            http: {
              metadata: {
                concurrentRequests: '20'
              }
            }
          }
        ]
      }
    }
  }
}

@description('The principal ID of the API container app managed identity')
output apiAppPrincipalId string = apiContainerApp.identity.principalId
