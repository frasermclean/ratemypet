targetScope = 'subscription'

extension microsoftGraphV1

@description('Name of the workload')
param workload string

@description('Application environments')
param appEnvironments array

@description('GitHub repository')
param gitHubRepository string

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

// service principal for the github actions application registration
resource servicePrincipal 'Microsoft.Graph/servicePrincipals@v1.0' = {
  appId: appRegistration.appId
}

@description('Service principal object ID. Can be used for role assignments')
output servicePrincipalId string = servicePrincipal.id
