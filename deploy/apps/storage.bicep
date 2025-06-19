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

var containerNames = [
  'post-images'
]

var queueNames = [
  'forgot-password'
  'post-added'
  'post-deleted'
  'register-confirmation'
]

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

    resource containers 'containers' = [
      for containerName in containerNames: {
        name: containerName
      }
    ]
  }

  resource queueServices 'queueServices' = {
    name: 'default'

    resource queues 'queues' = [
      for queueName in queueNames: {
        name: queueName
      }
    ]
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
