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

@description('Repository of the Web container image')
param webImageRepository string

@description('Tag of the Web container image')
param webImageTag string

@description('Attempt to bind to a managed certificate for the web app. Set to false on first deployment.')
param shouldBindManagedCertificate bool = true

var apiContainerAppName = '${workload}-${appEnv}-api-ca'
var webContainerAppName = '${workload}-${appEnv}-web-ca'

// shared managed identity
resource sharedIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31' existing = {
  name: '${workload}-shared-id'
  scope: resourceGroup(sharedResourceGroup)
}

// container apps environment
resource appsEnvironment 'Microsoft.App/managedEnvironments@2025-01-01' = {
  name: '${workload}-${appEnv}-cae'
  location: location
  tags: tags
  properties: {
    appLogsConfiguration: {
      destination: 'azure-monitor'
    }
  }

  // managed certificate for subdomain (web app)
  resource subdomainCertificate 'managedCertificates' = if (shouldBindManagedCertificate) {
    name: appEnv == 'prod' ? 'web-www-cert' : 'web-${appEnv}-cert'
    location: location
    tags: tags
    properties: {
      subjectName: dnsRecordsModule.outputs.webAppHostnames[0]
      domainControlValidation: 'CNAME'
    }
  }

  // managed certificate for apex domain (prod only)
  resource apexCertificate 'managedCertificates' = if (appEnv == 'prod' && shouldBindManagedCertificate) {
    name: 'web-apex-cert'
    location: location
    tags: tags
    properties: {
      subjectName: dnsRecordsModule.outputs.webAppHostnames[1]
      domainControlValidation: 'TXT'
    }
  }
}

// dns records (for custom domains)
module dnsRecordsModule 'dnsRecords.bicep' = {
  name: 'dnsRecords-${appEnv}-containerApps'
  scope: resourceGroup(sharedResourceGroup)
  params: {
    appEnv: appEnv
    domainName: domainName
    appEnvironmentIpAddress: appsEnvironment.properties.staticIp
    webAppDefaultHostname: '${webContainerAppName}.${appsEnvironment.properties.defaultDomain}'
    customDomainVerificationId: appsEnvironment.properties.customDomainConfiguration.customDomainVerificationId
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
        external: false
        targetPort: 8080
        allowInsecure: false
        traffic: [
          {
            latestRevision: true
            weight: 100
          }
        ]
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
              value: 'https://${appConfigurationName}.azconfig.io'
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

// web container app
resource webContainerApp 'Microsoft.App/containerApps@2024-03-01' = {
  name: webContainerAppName
  location: location
  tags: union(tags, { appName: 'web' })
  identity: {
    type: 'UserAssigned'
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
        customDomains: appEnv == 'prod'
          ? [
              {
                name: dnsRecordsModule.outputs.webAppHostnames[0]
                bindingType: shouldBindManagedCertificate ? 'SniEnabled' : 'Disabled'
                certificateId: shouldBindManagedCertificate ? appsEnvironment::subdomainCertificate.id : null
              }
              {
                name: dnsRecordsModule.outputs.webAppHostnames[1]
                bindingType: shouldBindManagedCertificate ? 'SniEnabled' : 'Disabled'
                certificateId: shouldBindManagedCertificate ? appsEnvironment::apexCertificate.id : null
              }
            ]
          : [
              {
                name: dnsRecordsModule.outputs.webAppHostnames[0]
                bindingType: shouldBindManagedCertificate ? 'SniEnabled' : 'Disabled'
                certificateId: shouldBindManagedCertificate ? appsEnvironment::subdomainCertificate.id : null
              }
            ]
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
          name: '${workload}-web'
          image: '${containerRegistryName}/${webImageRepository}:${webImageTag}'
          resources: {
            cpu: json('0.25')
            memory: '0.5Gi'
          }
          env: [
            {
              name: 'API_BASE_URL'
              value: 'https://${apiContainerApp.properties.configuration.ingress.fqdn}/api'
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

@description('Name of the API container app')
output apiAppName string = apiContainerApp.name

@description('The principal ID of the API container app managed identity')
output apiAppPrincipalId string = apiContainerApp.identity.principalId
