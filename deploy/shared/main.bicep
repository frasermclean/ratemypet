targetScope = 'resourceGroup'

@description('Name of the workload')
param workload string = 'ratemypet'

@description('Category of the workload')
param category string = 'shared'

@description('Location of the resources')
param location string = resourceGroup().location

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

param shouldCreateDataProtectionKeys bool = false

var tags = {
  workload: workload
  category: category
}

var environmentNames = ['dev', 'test', 'prod']

// DNS zone
resource dnsZone 'Microsoft.Network/dnsZones@2018-05-01' = {
  name: dnsZoneName
  location: 'global'
  tags: tags
  properties: {
    zoneType: 'Public'
  }

  // notify subdomain verification and spf records
  resource notifyDomainVerification 'TXT' = {
    name: 'notify'
    properties: {
      TTL: emailCommunicationServices::notifyDomain.properties.verificationRecords.Domain.ttl
      TXTRecords: [
        { value: [emailCommunicationServices::notifyDomain.properties.verificationRecords.Domain.value] }
        { value: [emailCommunicationServices::notifyDomain.properties.verificationRecords.SPF.value] }
      ]
    }
  }

  // notify subdomain dkim 1 record
  resource dkim1Record 'CNAME' = {
    name: 'selector1-azurecomm-prod-net._domainkey.notify'
    properties: {
      TTL: emailCommunicationServices::notifyDomain.properties.verificationRecords.DKIM.ttl
      CNAMERecord: {
        cname: emailCommunicationServices::notifyDomain.properties.verificationRecords.DKIM.value
      }
    }
  }

  // notify subdomain dkim 2 record
  resource dkim2Record 'CNAME' = {
    name: 'selector2-azurecomm-prod-net._domainkey.notify'
    properties: {
      TTL: emailCommunicationServices::notifyDomain.properties.verificationRecords.DKIM2.ttl
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

  resource dataProtectionKeys 'keys' = [
    for name in environmentNames: if (shouldCreateDataProtectionKeys) {
      name: 'data-protection-${name}'
      tags: {
        environment: name
      }
      properties: {
        kty: 'RSA'
        keySize: 2048
        keyOps: [
          'encrypt'
          'decrypt'
          'sign'
          'verify'
          'wrapKey'
          'unwrapKey'
        ]
        attributes: {
          enabled: true
          nbf: dateTimeToEpoch(now)
        }
      }
    }
  ]

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
  }

  resource emailSenderAddressKeyValue 'keyValues' = {
    name: 'Email:SenderAddress'
    properties: {
      value: 'no-reply@notify.${dnsZoneName}'
      contentType: 'text/plain'
    }
  }

  resource cloudinaryCloudNameKeyValue 'keyValues' = {
    name: 'Cloudinary:CloudName'
    properties: {
      value: 'ratemypet'
      contentType: 'text/plain'
    }
  }

  resource dataProtectionKeyUriKeyValues 'keyValues' = [
    for (name, i) in environmentNames: {
      name: 'DataProtection:KeyUri$${name}'
      properties: {
        value: keyVault::dataProtectionKeys[i].properties.keyUri
        contentType: 'text/plain'
      }
    }
  ]
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
