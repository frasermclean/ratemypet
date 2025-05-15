targetScope = 'resourceGroup'

@allowed(['prod', 'test'])
@description('Application environment')
param appEnv string

@description('Domain name')
param domainName string

@description('Name of the Azure App Configuration instance')
param appConfigurationName string

@description('Name of the Azure Key Vault instance')
param keyVaultName string

@description('Cloudinary API key')
param cloudinaryApiKey string = ''

@description('Storage account blob endpoint to be stored in App Configuration.')
param storageAccountBlobEndpoint string

@description('Storage account queue endpoint to be stored in App Configuration.')
param storageAccountQueueEndpoint string

@description('Database connection string to be stored in App Configuration.')
param databaseConnectionString string

@description('Computer Vision endpoint')
param computerVisionEndpoint string

@description('Content Safety endpoint')
param contentSafetyEndpoint string

resource appConfiguration 'Microsoft.AppConfiguration/configurationStores@2023-03-01' existing = {
  name: appConfigurationName

  resource aiServicesComputerVisionEndpointKeyValue 'keyValues' = {
    name: 'AiServices:ComputerVisionEndpoint$${appEnv}'
    properties: {
      value: computerVisionEndpoint
      contentType: 'text/plain'
    }
  }

  resource aiServicesContentSafetyEndpointKeyValue 'keyValues' = {
    name: 'AiServices:ContentSafetyEndpoint$${appEnv}'
    properties: {
      value: contentSafetyEndpoint
      contentType: 'text/plain'
    }
  }

  resource cloudinaryApiKeyKeyValue 'keyValues' = {
    name: 'Cloudinary:ApiKey$${appEnv}'
    properties: {
      value: cloudinaryApiKey
      contentType: 'text/plain'
    }
  }

  resource cloudinaryApiSecretKeyValue 'keyValues' = {
    name: 'Cloudinary:ApiSecret$${appEnv}'
    properties: {
      value: '{"uri":"https://${keyVaultName}${environment().suffixes.keyvaultDns}/secrets/cloudinary-api-secret-${appEnv}"}'
      contentType: 'application/vnd.microsoft.appconfig.keyvaultref+json;charset=utf-8'
    }
  }

  resource databaseConnectionStringKeyValue 'keyValues' = {
    name: 'ConnectionStrings:Database$${appEnv}'
    properties: {
      value: databaseConnectionString
      contentType: 'text/plain'
    }
  }

  resource storageBlobEndpointKeyValue 'keyValues' = {
    name: 'ConnectionStrings:Blobs$${appEnv}'
    properties: {
      value: storageAccountBlobEndpoint
      contentType: 'text/plain'
    }
  }

  resource storageQueueEndpointKeyValue 'keyValues' = {
    name: 'ConnectionStrings:Queues$${appEnv}'
    properties: {
      value: storageAccountQueueEndpoint
      contentType: 'text/plain'
    }
  }

  resource emailFrontendBaseUrlKeyValue 'keyValues' = {
    name: 'Email:FrontendBaseUrl$${appEnv}'
    properties: {
      value: appEnv == 'prod' ? 'https://${domainName}' : 'https://${appEnv}.${domainName}'
      contentType: 'text/plain'
    }
  }
}
