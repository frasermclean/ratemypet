targetScope = 'resourceGroup'

@description('Name of the Azure App Configuration instance')
param appConfigurationName string

@allowed(['prod', 'test'])
@description('Application environment')
param appEnv string

@description('Domain name')
param domainName string

@description('Storage account blob endpoint to be stored in App Configuration.')
param storageAccountBlobEndpoint string

@description('Storage account queue endpoint to be stored in App Configuration.')
param storageAccountQueueEndpoint string

@description('Database connection string to be stored in App Configuration.')
param databaseConnectionString string

resource appConfiguration 'Microsoft.AppConfiguration/configurationStores@2023-03-01' existing = {
  name: appConfigurationName

  resource databaseConnectionStringKeyValue 'keyValues' = {
    name: 'ConnectionStrings:Database$${appEnv}'
    properties: {
      value: databaseConnectionString
      contentType: 'text/plain'
    }
  }

  resource storageBlobEndpointKeyValue 'keyValues' = {
    name: 'Storage:BlobServiceUri$${appEnv}'
    properties: {
      value: storageAccountBlobEndpoint
      contentType: 'text/plain'
    }
  }

  resource storageQueueEndpointKeyValue 'keyValues' = {
    name: 'Storage:QueueServiceUri$${appEnv}'
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
