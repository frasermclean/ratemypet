targetScope = 'resourceGroup'

@description('Name of the key vault')
param keyVaultName string

@description('Array of prinicpal IDs that have administrative access to the key vault')
param keyVaultAdministrators array = []

@description('Array of prinicpal IDs that have read access to the key vault secrets')
param keyVaultSecretsUsers array = []

@description('Mapping of role names to role definition IDs')
var roleDefinitionIds = {
  KeyVaultAdministrator: '00482a5a-887f-4fb3-b363-3b7fe8e74483'
  KeyVaultSecretsUser: '4633458b-17de-408a-b874-0445c86b69e6'
}

// existing key vault to assign roles to
resource keyVault 'Microsoft.KeyVault/vaults@2023-07-01' existing = {
  name: keyVaultName
}

// key vault administrator role assignments
resource keyVaultAdministratorRoleAssigment 'Microsoft.Authorization/roleAssignments@2022-04-01' = [
  for principalId in keyVaultAdministrators: {
    name: guid(keyVault.id, roleDefinitionIds.KeyVaultAdministrator, principalId)
    scope: keyVault
    properties: {
      principalId: principalId
      roleDefinitionId: resourceId(
        'Microsoft.Authorization/roleDefinitions@2022-04-01',
        roleDefinitionIds.KeyVaultAdministrator
      )
    }
  }
]

// key vault secrets user role assignments
resource keyVaultSecretsUserRoleAssigment 'Microsoft.Authorization/roleAssignments@2022-04-01' = [
  for principalId in keyVaultSecretsUsers: {
    name: guid(keyVault.id, roleDefinitionIds.KeyVaultSecretsUser, principalId)
    scope: keyVault
    properties: {
      principalId: principalId
      roleDefinitionId: resourceId(
        'Microsoft.Authorization/roleDefinitions@2022-04-01',
        roleDefinitionIds.KeyVaultSecretsUser
      )
    }
  }
]
