targetScope = 'resourceGroup'

@allowed(['prod', 'test'])
@description('Environment of the application')
param appEnv string

@description('Domain name')
param domainName string = 'ratemy.pet'

@description('IP address of the Container Apps Environment')
param appEnvironmentIpAddress string = ''

@description('Default hostname of the web app')
param webAppDefaultHostname string = ''

@description('Default hostname of the jobs app')
param jobsAppDefaultHostname string = ''

@description('Custom domain verification ID')
param customDomainVerificationId string = ''

@description('TTL for DNS records in seconds')
param ttl int = 3600

resource dnsZone 'Microsoft.Network/dnsZones@2018-05-01' existing = {
  name: domainName

  // web app A record (prod only)
  resource webAppARecord 'A' = if (appEnv == 'prod' && !empty(appEnvironmentIpAddress)) {
    name: '@'
    properties: {
      TTL: ttl
      ARecords: [
        { ipv4Address: appEnvironmentIpAddress }
      ]
    }
  }

  // web app CNAME record
  resource webAppCnameRecord 'CNAME' = if (!empty(webAppDefaultHostname)) {
    name: appEnv == 'prod' ? 'www' : appEnv
    properties: {
      TTL: ttl
      CNAMERecord: {
        cname: webAppDefaultHostname
      }
    }
  }

  // web app subdomain TXT verification record
  resource webAppSubdomainTxtRecord 'TXT' = if (!empty(customDomainVerificationId)) {
    name: 'asuid.${webAppCnameRecord.name}'
    properties: {
      TTL: ttl
      TXTRecords: [
        { value: [customDomainVerificationId] }
      ]
    }
  }

  // web app apex domain TXT verification record (prod only)
  resource webAppApexTxtRecord 'TXT' = if (appEnv == 'prod' && !empty(customDomainVerificationId)) {
    name: 'asuid'
    properties: {
      TTL: ttl
      TXTRecords: [
        { value: [customDomainVerificationId] }
      ]
    }
  }

  // jobs app CNAME record
  resource jobsAppCnameRecord 'CNAME' = if (!empty(jobsAppDefaultHostname)) {
    name: 'jobs.${appEnv}'
    properties: {
      TTL: ttl
      CNAMERecord: {
        cname: jobsAppDefaultHostname
      }
    }
  }

  // jobs app TXT verification record
  resource jobsAppTxtRecord 'TXT' = if (!empty(customDomainVerificationId)) {
    name: 'asuid.jobs.${appEnv}'
    properties: {
      TTL: ttl
      TXTRecords: [
        { value: [customDomainVerificationId] }
      ]
    }
  }
}

@description('Fully qualified domain name of the web application')
output webAppHostnames array = appEnv == 'prod' ? ['www.${domainName}', domainName] : ['${appEnv}.${domainName}']
