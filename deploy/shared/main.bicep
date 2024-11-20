targetScope = 'resourceGroup'

@description('Name of the workload')
param workload string

@description('Category of the workload')
param category string

@description('Azure region of the non global resources')
param location string = resourceGroup().location

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
}

// email communication services
resource emailCommunicationServices 'Microsoft.Communication/emailServices@2023-04-01' = {
  name: '${workload}-${category}-ecs'
  location: 'global'
  tags: tags
  properties: {
    dataLocation: emailDataLocation
  }
}
