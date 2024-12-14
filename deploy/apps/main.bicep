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

@description('Application administrator group name')
param adminGroupName string

@description('Application administrator group object ID')
param adminGroupObjectId string

@description('Array of allowed external IP addresses. Needs to be an array of objects with name and ipAddress properties.')
param allowedExternalIpAddresses array = []

var tags = {
  workload: workload
  appEnv: appEnv
}

// azure sql database
module databaseModule 'database.bicep' = {
  name: 'database'
  params: {
    workload: workload
    appEnv: appEnv
    location: location
    tags: tags
    adminGroupName: adminGroupName
    adminGroupObjectId: adminGroupObjectId
    allowedExternalIpAddresses: allowedExternalIpAddresses
  }
}

// storage account
module storageModule 'storage.bicep' = {
  name: 'storage'
  params: {
    workload: workload
    appEnv: appEnv
    location: location
    tags: tags
  }
}

// frontend static web app
module staticWebAppModule 'staticWebApp.bicep' = {
  name: 'staticWebApp'
  params: {
    workload: workload
    appEnv: appEnv
    appName: 'frontend'
    location: 'eastasia'
    domainName: domainName
    sharedResourceGroup: sharedResourceGroup
    tags: tags
  }
}

// application insights
module appInsightsModule 'appInsights.bicep' = {
  name: 'appInsights'
  params: {
    workload: workload
    appEnv: appEnv
    location: location
    tags: tags
    actionGroupShortName: 'RMP - ${appEnv}'
  }
}
