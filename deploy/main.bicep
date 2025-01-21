targetScope = 'subscription'

@description('Name of the workload')
param workload string

@description('Location of the resource groups')
param location string

@description('Application environments')
param appEnvironments array

@description('GitHub repository')
param gitHubRepository string

@description('Application administrators group members object IDs')
param adminGroupMembers array = []

@description('If true, shared resources will be deployed')
param deploySharedResources bool = false

var tags = {
  workload: workload
}

var appEnvResourceGroups = [
  for env in appEnvironments: {
    resourceGroupName: '${workload}-${env}-rg'
    appEnv: env
  }
]

// shared resource group
resource sharedResourceGroup 'Microsoft.Resources/resourceGroups@2024-07-01' = {
  name: '${workload}-shared-rg'
  location: location
  tags: union(tags, {
    category: 'shared'
  })
}

// shared resources deployment
module sharedResources './shared/main.bicep' = if (deploySharedResources) {
  name: 'main'
  scope: sharedResourceGroup
  params: {
    workload: workload
    category: 'shared'
    location: location
    adminGroupObjectId: graphResources.outputs.adminGroupObjectId
    deploymentAppPrincipalId: graphResources.outputs.deploymentAppPrincipalId
  }
}

// application environment resource groups
resource appResourceGroups 'Microsoft.Resources/resourceGroups@2024-07-01' = [
  for rg in appEnvResourceGroups: {
    name: rg.resourceGroupName
    location: location
    tags: union(tags, {
      category: 'app'
      appEnv: rg.appEnv
    })
  }
]

// graph resources deployment
module graphResources './graphResources.bicep' = {
  name: '${deployment().name}-graphResources'
  params: {
    workload: workload
    appEnvironments: appEnvironments
    gitHubRepository: gitHubRepository
    adminGroupMembers: adminGroupMembers
  }
}

// resource group owner role assignments
module resourceGroupOwnerRoleAssignments './rgOwnerAssignment.bicep' = [
  for rg in concat(appEnvResourceGroups, [{ resourceGroupName: sharedResourceGroup.name }]): {
    name: 'resourceGroup-deploymentOwner'
    scope: resourceGroup(rg.resourceGroupName)
    params: {
      principalId: graphResources.outputs.deploymentAppPrincipalId
    }
  }
]
