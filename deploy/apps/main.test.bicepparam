using 'main.bicep'

param workload = 'ratemypet'
param appEnv = 'test'
param location = 'Southeast Asia'
param domainName = 'ratemy.pet'

// shared resources
param sharedResourceGroup = 'ratemypet-shared-rg'
param keyVaultName = 'ratemypet-shared-kv'
param appConfigurationName = 'ratemypet-shared-ac'

// admin group
param adminGroupName = 'Rate My Pet Administrators'
param adminGroupObjectId = 'e4721a75-4d03-4a91-8709-9cb50418d15d'

param allowedExternalIpAddresses = [
  {
    name: 'hive'
    ipAddress: readEnvironmentVariable('HIVE_IP_ADDRESS', '')
  }
]

// container apps
param apiImageRepository = readEnvironmentVariable('RMP_API_IMAGE_REPOSITORY', 'frasermclean/ratemypet-api')
param apiImageTag = readEnvironmentVariable('RMP_API_IMAGE_TAG', 'latest')
param containerRegistryName = readEnvironmentVariable('RMP_CONTAINER_REGISTRY', 'ghcr.io')
param containerRegistryUsername = readEnvironmentVariable('RMP_CONTAINER_REGISTRY_USERNAME', 'frasermclean')
