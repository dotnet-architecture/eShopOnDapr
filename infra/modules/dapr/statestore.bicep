param containerAppsEnvironmentName string

param cosmosDbName string
param cosmosCollectionName string
param cosmosUrl string
@secure()
param cosmosKey string

resource containerAppsEnvironment 'Microsoft.App/managedEnvironments@2022-03-01' existing = {
  name: containerAppsEnvironmentName

  resource daprComponent 'daprComponents@2022-03-01' = {
    name: 'eshopondapr-statestore'
    properties: {
      componentType: 'state.azure.cosmosdb'
      version: 'v1'
      secrets: [
        {
          name: 'cosmos-key'
          value: cosmosKey
        }
      ]
      metadata: [
        {
          name: 'url'
          value: cosmosUrl
        }
        {
          name: 'masterKey'
          secretRef: 'cosmos-key'
        }
        {
          name: 'database'
          value: cosmosDbName
        }
        {
          name: 'collection'
          value: cosmosCollectionName
        }
        {
          name: 'actorStateStore'
          value: 'true'
        }
      ]
      scopes: [
        'basket-api'
        'ordering-api'
      ]
    }
  }
}

output daprStateStoreName string = containerAppsEnvironment::daprComponent.name
