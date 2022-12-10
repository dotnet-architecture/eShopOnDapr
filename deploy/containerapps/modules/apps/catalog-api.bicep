param location string
param seqFqdn string

param containerAppsEnvironmentId string

param managedIdentityId string

@secure()
param catalogDbConnectionString string

resource containerApp 'Microsoft.App/containerApps@2022-03-01' = {
  name: 'catalog-api'
  location: location
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '${managedIdentityId}': {}
    }
  }
  properties: {
    managedEnvironmentId: containerAppsEnvironmentId
    template: {
      containers: [
        {
          name: 'catalog-api'
          image: 'eshopdapr/catalog.api:latest'
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
    }
    configuration: {
      activeRevisionsMode: 'single'
      dapr: {
        enabled: true
        appId: 'catalog-api'
        appPort: 80
      }
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
      ]
    }
  }
}
