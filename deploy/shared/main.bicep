targetScope = 'resourceGroup'

@description('Name of the workload')
param workload string

@description('Category of the workload')
param category string

@description('Domain name of the root DNS zone')
param dnsZoneName string

@description('Location of the email data')
param emailDataLocation string = 'Australia'

var tags = {
  workload: workload
  category: category
}

// root DNS zone
resource rootDnsZone 'Microsoft.Network/dnsZones@2018-05-01' = {
  name: dnsZoneName
  location: 'global'
  tags: tags
  properties: {
    zoneType: 'Public'
  }

  resource notifyNameserverRecords 'NS' = {
    name: 'notify'
    properties: {
      TTL: 3600
      NSRecords: [
        { nsdname: notifyDnsZone.properties.nameServers[0] }
        { nsdname: notifyDnsZone.properties.nameServers[1] }
        { nsdname: notifyDnsZone.properties.nameServers[2] }
        { nsdname: notifyDnsZone.properties.nameServers[3] }
      ]
    }
  }
}

// notify subdomain
resource notifyDnsZone 'Microsoft.Network/dnsZones@2018-05-01' = {
  name: 'notify.${dnsZoneName}'
  location: 'global'
  tags: tags
  properties: {
    zoneType: 'Public'
  }

  // notify domain verification and spf records
  resource notifyDomainVerification 'TXT' = {
    name: '@'
    properties: {
      TTL: 3600
      TXTRecords: [
        { value: [emailCommunicationServices::notifyDomain.properties.verificationRecords.Domain.value] }
        { value: [emailCommunicationServices::notifyDomain.properties.verificationRecords.SPF.value] }
      ]
    }
  }

  // dkim 1 record
  resource dkim1Record 'CNAME' = {
    name: 'selector1-azurecomm-prod-net._domainkey'
    properties: {
      TTL: 3600
      CNAMERecord: {
        cname: emailCommunicationServices::notifyDomain.properties.verificationRecords.DKIM.value
      }
    }
  }

  // dkim 2 record
  resource dkim2Record 'CNAME' = {
    name: 'selector2-azurecomm-prod-net._domainkey'
    properties: {
      TTL: 3600
      CNAMERecord: {
        cname: emailCommunicationServices::notifyDomain.properties.verificationRecords.DKIM2.value
      }
    }
  }
}

// email communication services
resource emailCommunicationServices 'Microsoft.Communication/emailServices@2023-04-01' = {
  name: '${workload}-${category}-ecs'
  location: 'global'
  tags: tags
  properties: {
    dataLocation: emailDataLocation
  }

  resource notifyDomain 'domains' = {
    name: 'notify.${dnsZoneName}'
    location: 'global'
    tags: tags
    properties: {
      domainManagement: 'CustomerManaged'
      userEngagementTracking: 'Disabled'
    }

    resource noReplyUsername 'senderUsernames' = {
      name: 'no-reply'
      properties: {
        username: 'no-reply'
        displayName: 'Rate My Pet'
      }
    }
  }
}

// communication services
resource communicationServices 'Microsoft.Communication/communicationServices@2023-04-01' = {
  name: '${workload}-${category}-acs'
  location: 'global'
  tags: tags
  properties: {
    dataLocation: emailDataLocation
    linkedDomains: [
      emailCommunicationServices::notifyDomain.id
    ]
  }
}
