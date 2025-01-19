using 'main.bicep'

param workload = 'ratemypet'
param category = 'shared'
param location = 'Australia East'
param dnsZoneName = 'ratemy.pet'
param emailDataLocation = 'Australia'

// container registry
param containerRegistryPassword = ''
param containerRegistryPasswordExpiry = ''

param adminGroupObjectId = 'e4721a75-4d03-4a91-8709-9cb50418d15d'
