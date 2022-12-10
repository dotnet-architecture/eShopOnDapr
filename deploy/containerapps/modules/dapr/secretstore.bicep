param containerAppsEnvironmentName string
param vaultName string
param managedIdentityClientId string

resource containerAppsEnvironment 'Microsoft.App/managedEnvironments@2022-03-01' existing = {
  name: containerAppsEnvironmentName

  resource daprComponent 'daprComponents@2022-03-01' = {
    name: 'eshopondapr-secretstore'
    properties: {
      componentType: 'secretstores.azure.keyvault'
      version: 'v1'
      secrets: [
        {
          name: 'azure-client-id'
          value: managedIdentityClientId
        }
      ]
      metadata: [
        {
          name: 'vaultName'
          value: vaultName
        }
        {
          name: 'azureEnvironment'
          value: 'AZUREPUBLICCLOUD'
        }
        {
          name: 'azureClientId'
          secretRef: 'azure-client-id'
        }
      ]
      scopes: [
        'catalog-api'
        'ordering-api'
        'identity-api'
      ]
    }
  }
}

output daprSecretStoreName string = containerAppsEnvironment::daprComponent.name
