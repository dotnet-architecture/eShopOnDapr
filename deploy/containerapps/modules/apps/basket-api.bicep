param location string
param seqFqdn string

param containerAppsEnvironmentId string
param containerAppsEnvironmentDomain string

param cosmosDbName string
param cosmosCollectionName string
param cosmosUrl string
@secure()
param cosmosKey string

@secure()
param serviceBusConnectionString string

resource containerApp 'Microsoft.Web/containerApps@2021-03-01' = {
  name: 'basket-api'
  location: location
  properties: {
    kubeEnvironmentId: containerAppsEnvironmentId
    template: {
      containers: [
        {
          name: 'basket-api'
          image: 'eshopdapr/basket.api:20220331'
          env: [
            {
              name: 'ASPNETCORE_ENVIRONMENT'
              value: 'Development'
            }
            {
              name: 'ASPNETCORE_URLS'
              value: 'http://0.0.0.0:80'
            }
            {
              name: 'IdentityUrl'
              value: 'https://identity-api.${containerAppsEnvironmentDomain}'
            }  
            {
              name: 'IdentityUrlExternal'
              value: 'https://identity-api.${containerAppsEnvironmentDomain}'
            }
            {
              name: 'SeqServerUrl'
              value: 'https://${seqFqdn}'
            }
          ]
        }
      ]
      scale: {
        minReplicas: 1
        maxReplicas: 1
      }
      dapr: {
        enabled: true
        appId: 'basket-api'
        appPort: 80
        components: [
          {
            name: 'eshop-statestore'
            type: 'state.azure.cosmosdb'
            version: 'v1'
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
            ]
          }
          {
            name: 'pubsub'
            type: 'pubsub.azure.servicebus'
            version: 'v1'
            metadata: [
              {
                name: 'connectionString'
                secretRef: 'service-bus-connection-string'
              }
            ]
            scopes: [
              'basket-api'
            ]
          }
        ]
      }
    }
    configuration: {
      activeResivionsMode: 'single'
      ingress: {
        external: false
        targetPort: 80
        allowInsecure: true
      }
      secrets: [
        {
          name: 'cosmos-key'
          value: cosmosKey
        }
        {
          name: 'service-bus-connection-string'
          value: serviceBusConnectionString
        }
      ]
    }
  }
}
