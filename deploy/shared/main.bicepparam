using 'main.bicep'

param workload = 'ratemypet'
param category = 'shared'
param location = 'Australia East'
param dnsZoneName = 'ratemy.pet'
param emailDataLocation = 'Australia'

// container registry
param containerRegistryPassword = ''
param containerRegistryPasswordExpiry = ''

// graph resources
param adminGroupObjectId = '0add1e4f-eec2-48cb-97fc-07911601323e'
param deploymentAppPrincipalId = '257de4b4-594b-4746-8452-3a52751bc5ed'
