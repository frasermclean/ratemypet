targetScope = 'resourceGroup'

@description('Name of the Azure App Configuration instance')
param appConfigurationName string

@allowed(['prod', 'test'])
@description('Application environment')
param appEnv string

@description('Storage account blob endpoint to be stored in App Configuration.')
param storageAccountBlobEndpoint string

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

  resource blobStorageConnectionStringKeyValue 'keyValues' = {
    name: 'Storage:BlobEndpoint$${appEnv}'
    properties: {
      value: storageAccountBlobEndpoint
      contentType: 'text/plain'
    }
  }
}
