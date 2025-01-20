targetScope = 'resourceGroup'

@description('Name of the workload')
param workload string = 'ratemypet'

@description('Category of the workload')
param category string = 'shared'

@description('Location of the resources')
param location string = resourceGroup().location

@allowed(['eastasia', 'centralus', 'eastus2', 'westeurope', 'westus2'])
@description('Location of the static web app')
param swaLocation string = 'eastasia'

@description('Domain name of the root DNS zone')
param dnsZoneName string = 'ratemy.pet'

@description('Location of the email data')
param emailDataLocation string = 'Australia'

@secure()
@description('Password for the container registry')
param containerRegistryPassword string = ''

@description('Expiry date for the container registry password in ISO 8601 format')
#disable-next-line secure-secrets-in-params
param containerRegistryPasswordExpiry string = ''

@description('Application administrator group object ID')
param adminGroupObjectId string

@description('Deployment app principal ID')
param deploymentAppPrincipalId string

@description('Current date and time in UTC')
param now string = utcNow()

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

  // static web app apex record
  resource apexARecord 'A' = {
    name: '@'
    properties: {
      TTL: 3600
      targetResource: {
        id: staticWebApp.id
      }
    }
  }

  // static web app www record
  resource wwwCnameRecord 'CNAME' = {
    name: 'www'
    properties: {
      TTL: 3600
      CNAMERecord: {
        cname: staticWebApp.properties.defaultHostname
      }
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

// static web app
resource staticWebApp 'Microsoft.Web/staticSites@2024-04-01' = {
  name: '${workload}-${category}-swa'
  location: swaLocation
  tags: tags
  sku: {
    name: 'Free'
  }
  properties: {
    stagingEnvironmentPolicy: 'Enabled'
    allowConfigFileUpdates: true
    buildProperties: {
      skipGithubActionWorkflowGeneration: true
    }
  }

  // apex custom domain
  resource apexCustomDomain 'customDomains' = {
    name: dnsZoneName
    dependsOn: [rootDnsZone::apexARecord]
    properties: {
      validationMethod: 'dns-txt-token'
    }
  }

  // www custom domain
  resource wwwCustomDomain 'customDomains' = {
    name: 'www.${dnsZoneName}'
    dependsOn: [rootDnsZone::wwwCnameRecord]
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

// key vault
resource keyVault 'Microsoft.KeyVault/vaults@2023-07-01' = {
  name: '${workload}-${category}-kv'
  location: location
  tags: tags
  properties: {
    enabledForTemplateDeployment: true
    enableRbacAuthorization: true
    tenantId: tenant().tenantId
    sku: {
      name: 'standard'
      family: 'A'
    }
  }

  resource containerRegistryPasswordSecret 'secrets' = if (!empty(containerRegistryPassword)) {
    name: 'container-registry-password'
    properties: {
      value: containerRegistryPassword
      contentType: 'text/plain'
      attributes: {
        enabled: true
        nbf: dateTimeToEpoch(now)
        exp: dateTimeToEpoch(containerRegistryPasswordExpiry)
      }
    }
  }
}

// app configuration
resource appConfiguration 'Microsoft.AppConfiguration/configurationStores@2024-05-01' = {
  name: '${workload}-shared-ac'
  location: location
  tags: tags
  sku: {
    name: 'Free'
  }
  properties: {
    disableLocalAuth: true
    dataPlaneProxy: {
      authenticationMode: 'Pass-through'
    }
  }

  resource emailAcsEndpointKeyValue 'keyValues' = {
    name: 'Email:AcsEndpoint'
    properties: {
      value: 'https://${communicationServices.properties.hostName}'
      contentType: 'text/plain'
    }
    dependsOn: [roleAssignments]
  }

  resource emailSenderAddressKeyValue 'keyValues' = {
    name: 'Email:SenderAddress'
    properties: {
      value: 'no-reply@notify.${dnsZoneName}'
      contentType: 'text/plain'
    }
    dependsOn: [roleAssignments]
  }

  resource emailFrontendBaseUrlKeyValue 'keyValues' = {
    name: 'Email:FrontendBaseUrl$dev'
    properties: {
      value: 'http://localhost:4200'
      contentType: 'text/plain'
    }
    dependsOn: [roleAssignments]
  }

  resource storageBlobEndpointKeyValue 'keyValues' = {
    name: 'Storage:BlobEndpoint$dev'
    properties: {
      value: 'https://localhost:10000/devstoreaccount1'
      contentType: 'text/plain'
    }
    dependsOn: [roleAssignments]
  }

  resource storageQueueEndpointKeyValue 'keyValues' = {
    name: 'Storage:QueueEndpoint$dev'
    properties: {
      value: 'https://localhost:10001/devstoreaccount1'
      contentType: 'text/plain'
    }
    dependsOn: [roleAssignments]
  }
}

// managed identity
resource managedIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31' = {
  name: '${workload}-${category}-id'
  location: location
  tags: tags
}

// role assignments
module roleAssignments './roleAssignments.bicep' = {
  name: 'roleAssignments'
  params: {
    keyVaultAdministrators: [adminGroupObjectId]
    keyVaultSecretsUsers: [managedIdentity.properties.principalId]
    configurationDataOwners: [adminGroupObjectId, deploymentAppPrincipalId]
    communicationAndEmailServiceOwners: [adminGroupObjectId]
  }
}

@description('Name of the key vault')
output keyVaultName string = keyVault.name

@description('Name of the shared app configuration')
output appConfigurationName string = appConfiguration.name
