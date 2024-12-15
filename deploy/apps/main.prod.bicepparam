using 'main.bicep'

param workload = 'ratemypet'
param appEnv = 'prod'
param location = 'Australia East'
param domainName = 'ratemy.pet'
param sharedResourceGroup = 'ratemypet-shared-rg'

param adminGroupName = 'Rate My Pet Administrators'
param adminGroupObjectId = 'e4721a75-4d03-4a91-8709-9cb50418d15d'

param allowedExternalIpAddresses = [
  {
    name: 'hive'
    ipAddress: readEnvironmentVariable('HIVE_IP_ADDRESS', '')
  }
]
