targetScope = 'resourceGroup'

@allowed(['prod', 'test'])
@description('Environment of the application')
param appEnv string

@description('Domain name')
param domainName string

@description('Resource ID of the static web app')
param swaResourceId string

@description('Default hostname of the static web app')
param swaDefaultHostname string = ''

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
}
