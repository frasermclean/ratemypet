@minLength(3)
@description('Name of the workload')
param workload string

@allowed(['prod', 'test', 'dev'])
@description('Application environment')
param appEnv string

@description('Azure region for the non-global resources')
param location string = resourceGroup().location

@description('Tags for the resources')
param tags object = {
  workload: workload
  appEnv: appEnv
}

// ai services account
resource aiServices 'Microsoft.CognitiveServices/accounts@2024-10-01' = {
  name: '${workload}-${appEnv}-ais'
  location: location
  tags: tags
  kind: 'AIServices'
  sku: {
    name: 'S0'
  }
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    customSubDomainName: '${workload}-${appEnv}'
    disableLocalAuth: true
    publicNetworkAccess: 'Enabled'
  }
}

@description('The endpoint for the Computer Vision service')
output computerVisionEndpoint string = aiServices.properties.endpoints['Computer Vision']

@description('The endpoint for the Content Safety service')
output contentSafetyEndpoint string = aiServices.properties.endpoints['Content Safety']

@description('The principal ID of the AI Services account')
output aiServicesIdentityPrincipalId string = aiServices.identity.principalId
