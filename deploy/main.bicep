targetScope = 'subscription'

@description('Name of the workload')
param workload string

@description('Location of the resource groups')
param location string

@description('Application environments')
param appEnvironments array

@description('GitHub repository')
param gitHubRepository string

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

// deployment application registration
module deploymentApp './deploymentApp.bicep' = {
  name: '${deployment().name}-deploymentApp'
  params: {
    workload: workload
    appEnvironments: appEnvironments
    gitHubRepository: gitHubRepository
  }
}

// resource group owner role assignments
module resourceGroupOwnerRoleAssignments './rgOwnerAssignment.bicep' = [
  for rg in concat(appEnvResourceGroups, [{ resourceGroupName: sharedResourceGroup.name }]): {
    name: 'resourceGroup-deploymentOwner'
    scope: resourceGroup(rg.resourceGroupName)
    params: {
      principalId: deploymentApp.outputs.servicePrincipalId
    }
  }
]
