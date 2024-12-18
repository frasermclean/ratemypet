targetScope = 'resourceGroup'

@allowed(['prod', 'test'])
@description('Environment of the application')
param appEnv string

@description('Domain name')
param domainName string = 'ratemy.pet'

@description('Resource ID of the static web app')
param swaResourceId string = ''

@description('Default hostname of the static web app')
param swaDefaultHostname string = ''

@description('Default hostname of the API app')
param apiAppDefaultHostname string = ''

@description('Default hostname of the jobs app')
param jobsAppDefaultHostname string = ''

@description('Custom domain verification ID')
param customDomainVerificationId string = ''

resource dnsZone 'Microsoft.Network/dnsZones@2018-05-01' existing = {
  name: domainName

  // static web app A record (apex only)
  resource staticWebAppARecord 'A' = if (appEnv == 'prod' && !empty(swaResourceId)) {
    name: '@'
    properties: {
      TTL: 3600
      targetResource: {
        id: swaResourceId
      }
    }
  }

  // static web app CNAME record (subdomain only)
  resource swaCnameRecord 'CNAME' = if (appEnv != 'prod' && !empty(swaDefaultHostname)) {
    name: appEnv
    properties: {
      TTL: 3600
      CNAMERecord: {
        cname: swaDefaultHostname
      }
    }
  }

  // www CNAME record (prod only)
  resource wwwCnameRecord 'CNAME' = if (appEnv == 'prod' && !empty(swaDefaultHostname)) {
    name: 'www'
    properties: {
      TTL: 3600
      CNAMERecord: {
        cname: swaDefaultHostname
      }
    }
  }

  // API app CNAME record
  resource apiAppCnameRecord 'CNAME' = if (!empty(apiAppDefaultHostname)) {
    name: appEnv == 'prod' ? 'api' : 'api.${appEnv}'
    properties: {
      TTL: 3600
      CNAMERecord: {
        cname: apiAppDefaultHostname
      }
    }
  }

  // API app TXT verification record
  resource apiAppTxtRecord 'TXT' = if (!empty(customDomainVerificationId)) {
    name: appEnv == 'prod' ? 'asuid.api' : 'asuid.api.${appEnv}'
    properties: {
      TTL: 3600
      TXTRecords: [
        { value: [customDomainVerificationId] }
      ]
    }
  }

  // jobs app CNAME record
  resource jobsAppCnameRecord 'CNAME' = if (!empty(jobsAppDefaultHostname)) {
    name: 'jobs.${appEnv}'
    properties: {
      TTL: 3600
      CNAMERecord: {
        cname: jobsAppDefaultHostname
      }
    }
  }

  // jobs app TXT verification record
  resource jobsAppTxtRecord 'TXT' = if (!empty(customDomainVerificationId)) {
    name: 'asuid.jobs.${appEnv}'
    properties: {
      TTL: 3600
      TXTRecords: [
        { value: [customDomainVerificationId] }
      ]
    }
  }
}

@description('Fully qualified domain name of the API application')
output apiAppFqdn string = '${dnsZone::apiAppCnameRecord.name}.${domainName}'
