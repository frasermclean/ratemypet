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

resource appConfiguration 'Microsoft.AppConfiguration/configurationStores@2023-03-01' existing = {
  name: appConfigurationName

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
    name: 'Storage:BlobEndpoint$${appEnv}'
    properties: {
      value: storageAccountBlobEndpoint
      contentType: 'text/plain'
    }
  }

  resource storageQueueEndpointKeyValue 'keyValues' = {
    name: 'Storage:QueueEndpoint$${appEnv}'
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
