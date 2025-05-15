targetScope = 'resourceGroup'

@minLength(3)
@description('Name of the workload')
param workload string = 'adoptrix'

@minLength(3)
@description('Environment of the workload')
param appEnv string = 'demo'

@minLength(3)
@description('Name of the application')
param appName string = 'jobs'

@description('Azure region for the non-global resources')
param location string = resourceGroup().location

@description('Domain name')
param domainName string

@description('Name of the shared resource group')
param sharedResourceGroup string

param keyVaultName string = 'ratemypet-shared-kv'

@description('Name of the Communication Service to use for the function app')
param communicationServiceName string = 'ratemypet-shared-acs'

@description('Name of the Azure Storage Account to use for the function app')
param storageAccountName string

@description('Database connection string')
param databaseConnectionString string

@description('Application Insights connection string')
param applicationInsightsConnectionString string

@description('Computer Vision endpoint')
param computerVisionEndpoint string

@description('Content Safety endpoint')
param contentSafetyEndpoint string

@description('Cloudinary API key')
param cloudinaryApiKey string

var tags = {
  workload: workload
  appEnv: appEnv
  appName: appName
}

var customDomainName = 'jobs.${appEnv}.${domainName}'

resource storageAccount 'Microsoft.Storage/storageAccounts@2023-05-01' existing = {
  name: storageAccountName

  resource blobServices 'blobServices' = {
    name: 'default'

    resource deploymentContainer 'containers' = {
      name: '${appName}-deployment'
    }
  }
}

// flex consumption app service plan
resource appServicePlan 'Microsoft.Web/serverfarms@2024-04-01' = {
  name: '${workload}-${appEnv}-${appName}-asp'
  location: location
  tags: tags
  kind: 'linux'
  sku: {
    name: 'FC1'
    tier: 'FlexConsumption'
  }
  properties: {
    reserved: true
  }
}

// function app
resource functionApp 'Microsoft.Web/sites@2024-04-01' = {
  name: '${workload}-${appEnv}-${appName}-func'
  location: location
  tags: tags
  kind: 'functionapp,linux'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    publicNetworkAccess: 'Enabled'
    siteConfig: {
      appSettings: [
        {
          name: 'AzureWebJobsStorage__accountName'
          value: storageAccount.name
        }
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: applicationInsightsConnectionString
        }
        {
          name: 'Email__AcsEndpoint'
          value: 'https://${communicationServiceName}.australia.communication.azure.com'
        }
        {
          name: 'Email__FrontendBaseUrl'
          value: appEnv == 'prod' ? 'https://ratemy.pet' : 'https://${appEnv}.ratemy.pet'
        }
        {
          name: 'Email__SenderAddress'
          value: 'no-reply@notify.ratemy.pet'
        }
        {
          name: 'AiServices__ComputerVisionEndpoint'
          value: computerVisionEndpoint
        }
        {
          name: 'AiServices__ContentSafetyEndpoint'
          value: contentSafetyEndpoint
        }
        {
          name: 'Cloudinary__CloudName'
          value: 'ratemypet'
        }
        {
          name: 'Cloudinary__ApiKey'
          value: cloudinaryApiKey
        }
        {
          name: 'Cloudinary__ApiSecret'
          value: '@Microsoft.KeyVault(VaultName=${keyVaultName};SecretName=cloudinary-api-secret-${appEnv})'
        }
      ]
      connectionStrings: [
        {
          name: 'Database'
          type: 'SQLAzure'
          connectionString: databaseConnectionString
        }
        {
          name: 'Blobs'
          type: 'Custom'
          connectionString: storageAccount.properties.primaryEndpoints.blob
        }
        {
          name: 'Queues'
          type: 'Custom'
          connectionString: storageAccount.properties.primaryEndpoints.queue
        }
      ]
      cors: {
        allowedOrigins: [
          'https://portal.azure.com'
        ]
      }
    }
    functionAppConfig: {
      deployment: {
        storage: {
          type: 'blobContainer'
          value: '${storageAccount.properties.primaryEndpoints.blob}${storageAccount::blobServices::deploymentContainer.name}'
          authentication: {
            type: 'SystemAssignedIdentity'
          }
        }
      }
      scaleAndConcurrency: {
        maximumInstanceCount: 40
        instanceMemoryMB: 2048
      }
      runtime: {
        name: 'dotnet-isolated'
        version: '9.0'
      }
    }
  }

  // custom domain binding
  resource hostNameBinding 'hostNameBindings' = {
    name: customDomainName
    dependsOn: [dnsRecords]
    properties: {
      hostNameType: 'Verified'
      sslState: 'Disabled'
      customHostNameDnsRecordType: 'CName'
    }
  }

  resource scmPublishingPolicy 'basicPublishingCredentialsPolicies' = {
    name: 'scm'
    properties: {
      allow: true
    }
  }

  resource ftpPublishingPolicy 'basicPublishingCredentialsPolicies' = {
    name: 'ftp'
    properties: {
      allow: false
    }
  }
}

// dns records
module dnsRecords 'dnsRecords.bicep' = {
  name: 'dnsRecords-${appEnv}-jobs'
  scope: resourceGroup(sharedResourceGroup)
  params: {
    domainName: domainName
    appEnv: appEnv
    jobsAppDefaultHostname: functionApp.properties.defaultHostName
    customDomainVerificationId: functionApp.properties.customDomainVerificationId
  }
}

// managed certificate
resource customDomainCertificate 'Microsoft.Web/certificates@2023-12-01' = {
  name: 'jobs-cert'
  location: location
  properties: {
    serverFarmId: appServicePlan.id
    canonicalName: functionApp::hostNameBinding.name
  }
}

// enable SNI binding for the custom domain
module siteSniEnable './siteSniEnable.bicep' = {
  name: 'siteSniEnable'
  params: {
    siteName: functionApp.name
    hostname: customDomainName
    certificateThumbprint: customDomainCertificate.properties.thumbprint
  }
}

@description('The name of the function app')
output appName string = functionApp.name

@description('The principal ID of the function app managed identity')
output principalId string = functionApp.identity.principalId
