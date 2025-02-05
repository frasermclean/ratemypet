@minLength(3)
@description('Name of the workload')
param workload string

@allowed(['prod', 'test'])
@description('Application environment')
param appEnv string

@allowed([
  'eastasia'
  'centralus'
  'eastus2'
  'westeurope'
  'westus2'
])
@description('Location of the static web app')
param location string

@minLength(3)
@description('Name of the application')
param appName string

@description('Tags for the resources')
param tags object = {
  workload: workload
  appEnv: appEnv
}

@description('Domain name')
param domainName string

@description('Name of the shared resource group')
param sharedResourceGroup string

@description('String to append to the end of deployment names')
param deploymentSuffix string = ''

// static web app
resource staticWebApp 'Microsoft.Web/staticSites@2024-04-01' = {
  name: '${workload}-${appEnv}-${appName}-swa'
  location: location
  tags: union(tags, {
    appName: appName
  })
  sku: {
    name: 'Free'
  }
  properties: {
    stagingEnvironmentPolicy: 'Disabled'
    allowConfigFileUpdates: true
    buildProperties: {
      skipGithubActionWorkflowGeneration: true
    }
  }

  // primary custom domain
  resource customDomain 'customDomains' = {
    name: appEnv == 'prod' ? domainName : '${appEnv}.${domainName}'
    dependsOn: [dnsRecordsModule]
    properties: {
      validationMethod: appEnv == 'prod' ? 'dns-txt-token' : null
    }
  }

  // www custom domain (prod only)
  resource wwwCustomDomain 'customDomains' = if (appEnv == 'prod') {
    name: 'www.${domainName}'
    dependsOn: [dnsRecordsModule]
  }
}

module dnsRecordsModule 'dnsRecords.bicep' = {
  name: 'dnsRecords-${appEnv}-${appName}${deploymentSuffix}'
  scope: resourceGroup(sharedResourceGroup)
  params: {
    domainName: domainName
    appEnv: appEnv
    swaResourceId: staticWebApp.id
    swaDefaultHostname: staticWebApp.properties.defaultHostname
  }
}

output staticWebAppName string = staticWebApp.name

@description('Hostnames at which the static web app is accessible')
output hostnames string[] = concat([staticWebApp.properties.defaultHostname], staticWebApp.properties.customDomains)
