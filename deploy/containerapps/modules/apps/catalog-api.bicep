param location string
param seqFqdn string

param containerAppsEnvironmentId string

@secure()
param catalogDbConnectionString string

@secure()
param serviceBusConnectionString string

resource containerApp 'Microsoft.Web/containerApps@2021-03-01' = {
  name: 'catalog-api'
  location: location
  properties: {
    kubeEnvironmentId: containerAppsEnvironmentId
    template: {
      containers: [
        {
          name: 'catalog-api'
          image: 'eshopdapr/catalog.api:20220331'
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
              name: 'ConnectionStrings__CatalogDB'
              secretRef: 'catalogdb-connection-string'
            }
            {
              name: 'RetryMigrations'
              value: 'true'
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
        appId: 'catalog-api'
        appPort: 80
        components: [
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
              'catalog-api'
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
          name: 'catalogdb-connection-string'
          value: catalogDbConnectionString
        }
        {
          name: 'service-bus-connection-string'
          value: serviceBusConnectionString
        }
      ]
    }
  }
}
