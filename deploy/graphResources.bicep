targetScope = 'subscription'

extension microsoftGraphV1

@description('Name of the workload')
param workload string

@description('Application environments')
param appEnvironments array

@description('GitHub repository')
param gitHubRepository string

@description('Application administrators group members object IDs')
param adminGroupMembers array = []

// application administrators group
resource adminGroup 'Microsoft.Graph/groups@v1.0' = {
  displayName: 'Rate My Pet Administrators'
  uniqueName: '${workload}-administrators'
  description: 'Administrators for Rate My Pet application'
  mailEnabled: false
  mailNickname: 'ratemypet-administrators'
  members: adminGroupMembers
  securityEnabled: true
}

// deployment application registration
resource appRegistration 'Microsoft.Graph/applications@v1.0' = {
  displayName: 'Rate My Pet Deployment'
  uniqueName: '${workload}-deployment'
  description: 'Application registration for deployment automation'

  // github actions credentials for each app environment
  @batchSize(1)
  resource appEnvCredentials 'federatedIdentityCredentials' = [
    for appEnv in appEnvironments: {
      name: '${appRegistration.uniqueName}/gha-${appEnv}-creds'
      description: 'GitHub Actions ${appEnv} environment credentials'
      issuer: 'https://token.actions.githubusercontent.com'
      subject: 'repo:${gitHubRepository}:environment:${appEnv}'
      audiences: ['api://AzureADTokenExchange']
    }
  ]
}

// service principal for the deployment application
resource servicePrincipal 'Microsoft.Graph/servicePrincipals@v1.0' = {
  appId: appRegistration.appId
}

@description('Administrators group object ID')
output adminGroupObjectId string = adminGroup.id

@description('Service principal object ID for the deployment application')
output deploymentAppPrincipalId string = servicePrincipal.id
