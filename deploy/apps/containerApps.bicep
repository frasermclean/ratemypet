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

@description('Tags for the resources')
param tags object = {
  workload: workload
  appEnv: appEnv
}

@description('Resource ID of the Log Analytics workspace')
param logAnalyticsWorkspaceId string

var apiContainerAppName = '${workload}-${appEnv}-api-ca'

// container apps environment
resource appsEnvironment 'Microsoft.App/managedEnvironments@2024-03-01' = {
  name: '${workload}-${appEnv}-cae'
  location: location
  tags: tags
  properties: {
    appLogsConfiguration: {
      destination: 'azure-monitor'
    }
  }
}

// container apps environment diagnostic settings
resource appsEnvironmentDiagnosticSettings 'Microsoft.Insights/diagnosticSettings@2021-05-01-preview' = {
  name: 'log-analytics'
  scope: appsEnvironment
  properties: {
    workspaceId: logAnalyticsWorkspaceId
    logs: [
      {
        category: 'ContainerAppSystemLogs'
        enabled: true
      }
    ]
  }
}

module dnsRecordsModule 'dnsRecords.bicep' = {
  name: 'dnsRecords-${appEnv}-containerApps'
  scope: resourceGroup(sharedResourceGroup)
  params: {
    appEnv: appEnv
    domainName: domainName
    apiAppDefaultHostname: '${apiContainerAppName}.${appsEnvironment.properties.defaultDomain}'
    caeDomainVerificationId: appsEnvironment.properties.customDomainConfiguration.customDomainVerificationId
  }
}
