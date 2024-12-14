targetScope = 'resourceGroup'

/*
This template defines Azure resources for use in a development environment.
*/

@description('Name of the workload')
param workload string = 'ratemypet'

@description('Application environment')
param appEnv string = 'dev'

@description('Azure region for the non-global resources')
param location string = resourceGroup().location

@description('Principal IDs to be assigned the Monitoring Metrics Publisher role')
param principalIds array = []

module appInsightsModule './apps/appInsights.bicep' = {
  name: 'appInsights'
  params: {
    workload: workload
    appEnv: appEnv
    location: location
    monitoringMetricsPublishers: principalIds
    actionGroupShortName: 'RMP - ${appEnv}'
  }
}

output applicationInsightsConnectionString string = appInsightsModule.outputs.connectionString
