@minLength(3)
@description('Name of the workload')
param workload string = 'ratemypet'

@allowed(['prod', 'test'])
@description('Application environment')
param appEnv string

@description('Azure region for the non-global resources')
param location string = resourceGroup().location

@description('Domain name')
param domainName string = 'ratemy.pet'

@description('Name of the shared resource group')
param sharedResourceGroup string = 'ratemypet-shared-rg'

@description('Name of the Azure Key Vault instance')
param keyVaultName string = 'ratemypet-shared-kv'

@description('Name of the Azure App Configuration instance')
param appConfigurationName string = 'ratemypet-shared-ac'

@description('Application administrator group name')
param adminGroupName string = 'Rate My Pet Administrators'

@description('Application administrator group object ID')
param adminGroupObjectId string = '0add1e4f-eec2-48cb-97fc-07911601323e'

@description('Array of allowed external IP addresses. Needs to be an array of objects with name and ipAddress properties.')
param allowedExternalIpAddresses array = []

@description('Repository of the API container image')
param apiImageRepository string = 'frasermclean/ratemypet-api'

@description('Tag of the API container image')
param apiImageTag string = 'latest'

@description('Repository of the Web container image')
param webImageRepository string = 'frasermclean/ratemypet-web'

@description('Tag of the Web container image')
param webImageTag string = 'latest'

@description('Container registry login server')
param containerRegistryName string = 'ghcr.io'

@description('Username to access the container registry')
param containerRegistryUsername string = 'frasermclean'

@description('Cloudinary API key')
param cloudinaryApiKey string

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
    webImageRepository: webImageRepository
    webImageTag: webImageTag
    containerRegistryName: containerRegistryName
    containerRegistryUsername: containerRegistryUsername
    keyVaultName: keyVaultName
  }
}

// jobs function app
module jobsAppModule './functionApp.bicep' = {
  name: 'functionApp'
  params: {
    workload: workload
    appEnv: appEnv
    appName: 'jobs'
    location: location
    domainName: domainName
    sharedResourceGroup: sharedResourceGroup
    storageAccountName: storageModule.outputs.accountName
    keyVaultName: keyVaultName
    databaseConnectionString: databaseModule.outputs.connectionString
    applicationInsightsConnectionString: appInsightsModule.outputs.connectionString
    computerVisionEndpoint: aiServicesModule.outputs.computerVisionEndpoint
    contentSafetyEndpoint: aiServicesModule.outputs.contentSafetyEndpoint
    cloudinaryApiKey: cloudinaryApiKey
  }
}

// ai services
module aiServicesModule './aiServices.bicep' = {
  name: 'aiServices'
  params: {
    workload: workload
    appEnv: appEnv
    location: location
    tags: tags
  }
}

// app configuration
module appConfigModule 'appConfig.bicep' = {
  name: 'appConfig-${appEnv}'
  scope: resourceGroup(sharedResourceGroup)
  params: {
    appEnv: appEnv
    appConfigurationName: appConfigurationName
    keyVaultName: keyVaultName
    domainName: domainName
    cloudinaryApiKey: cloudinaryApiKey
    databaseConnectionString: databaseModule.outputs.connectionString
    storageAccountBlobEndpoint: storageModule.outputs.blobEndpoint
    storageAccountQueueEndpoint: storageModule.outputs.queueEndpoint
    computerVisionEndpoint: aiServicesModule.outputs.computerVisionEndpoint
    contentSafetyEndpoint: aiServicesModule.outputs.contentSafetyEndpoint
  }
}

// role assignments
module roleAssignmentsModule 'roleAssignments.bicep' = {
  name: 'roleAssignments'
  params: {
    adminGroupObjectId: adminGroupObjectId
    apiAppPrincipalId: containerAppsModule.outputs.apiAppPrincipalId
    jobsAppPrincipalId: jobsAppModule.outputs.principalId
    storageAccountName: storageModule.outputs.accountName
    applicationInsightsName: appInsightsModule.outputs.applicationInsightsName
    aiServicesName: aiServicesModule.outputs.aiServicesName
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
      jobsAppModule.outputs.principalId
    ]
    appConfigurationName: appConfigurationName
    configurationDataReaders: [
      containerAppsModule.outputs.apiAppPrincipalId
      jobsAppModule.outputs.principalId
    ]
    communicationAndEmailServiceOwners: [
      jobsAppModule.outputs.principalId
    ]
  }
}

@description('Name of the API container app')
output apiAppName string = containerAppsModule.outputs.apiAppName

@description('Name of the jobs function app')
output jobsAppName string = jobsAppModule.outputs.appName
