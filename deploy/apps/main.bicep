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

@description('Name of the Azure Key Vault instance')
param keyVaultName string

@description('Name of the Azure App Configuration instance')
param appConfigurationName string

@description('Application administrator group name')
param adminGroupName string

@description('Application administrator group object ID')
param adminGroupObjectId string

@description('Array of allowed external IP addresses. Needs to be an array of objects with name and ipAddress properties.')
param allowedExternalIpAddresses array = []

@description('Repository of the API container image')
param apiImageRepository string

@description('Tag of the API container image')
param apiImageTag string

@description('Container registry login server')
param containerRegistryName string

@description('Username to access the container registry')
param containerRegistryUsername string

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

// container apps
module containerAppsModule 'containerApps.bicep' = {
  name: 'containerApps'
  params: {
    workload: workload
    appEnv: appEnv
    location: location
    tags: tags
    domainName: domainName
    sharedResourceGroup: sharedResourceGroup
    logAnalyticsWorkspaceId: appInsightsModule.outputs.logAnalyticsWorkspaceId
    applicationInsightsConnectionString: appInsightsModule.outputs.connectionString
    appConfigurationName: appConfigurationName
    apiImageRepository: apiImageRepository
    apiImageTag: apiImageTag
    apiAllowedOrigins: map(staticWebAppModule.outputs.hostnames, (hostname) => 'https://${hostname}')
    containerRegistryName: containerRegistryName
    containerRegistryUsername: containerRegistryUsername
    keyVaultName: keyVaultName
  }
}

// app configuration
module appConfigModule 'appConfig.bicep' = {
  name: 'appConfig-${appEnv}'
  scope: resourceGroup(sharedResourceGroup)
  params: {
    appEnv: appEnv
    appConfigurationName: appConfigurationName
    domainName: domainName
    databaseConnectionString: databaseModule.outputs.connectionString
    storageAccountBlobEndpoint: storageModule.outputs.blobEndpoint
  }
}

// role assignments
module roleAssignmentsModule 'roleAssignments.bicep' = {
  name: 'roleAssignments'
  params: {
    adminGroupObjectId: adminGroupObjectId
    apiAppPrincipalId: containerAppsModule.outputs.apiAppPrincipalId
    storageAccountName: storageModule.outputs.accountName
    applicationInsightsName: appInsightsModule.outputs.applicationInsightsName
  }
}

// shared resource role assignments
module sharedRoleAssignmentsModule '../shared/roleAssignments.bicep' = {
  name: 'roleAssignments-${appEnv}'
  scope: resourceGroup(sharedResourceGroup)
  params: {
    keyVaultName: keyVaultName
    keyVaultSecretsUsers: [
      containerAppsModule.outputs.apiAppPrincipalId
    ]
    appConfigurationName: appConfigurationName
    configurationDataReaders: [
      containerAppsModule.outputs.apiAppPrincipalId
    ]
  }
}
