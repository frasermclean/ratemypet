@minLength(3)
@description('Name of the workload')
param workload string

@allowed(['prod', 'test'])
@description('Application environment')
param appEnv string

@description('Azure region for the non-global resources')
param location string = resourceGroup().location

@description('Tags for the resources')
param tags object = {
  workload: workload
  appEnv: appEnv
}

// storage account
resource storageAccount 'Microsoft.Storage/storageAccounts@2023-01-01' = {
  name: '${workload}${appEnv}'
  location: location
  tags: tags
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
  properties: {
    allowBlobPublicAccess: true
    allowSharedKeyAccess: true
    defaultToOAuthAuthentication: true
    minimumTlsVersion: 'TLS1_2'
  }

  resource blobServices 'blobServices' = {
    name: 'default'

    resource postImagesContainer 'containers' = {
      name: 'post-images'
      properties: {
        publicAccess: 'Blob'
      }
    }

    resource originalImagesContainer 'containers' = {
      name: 'original-images'
    }
  }
}

@description('Name of the storage account')
output accountName string = storageAccount.name

@description('Blob endpoint')
output blobEndpoint string = storageAccount.properties.primaryEndpoints.blob

@description('Table endpoint')
output tableEndpoint string = storageAccount.properties.primaryEndpoints.table

@description('Queue endpoint')
output queueEndpoint string = storageAccount.properties.primaryEndpoints.queue
