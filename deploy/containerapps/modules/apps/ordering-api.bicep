param location string
param seqFqdn string

param containerAppsEnvironmentId string
param containerAppsEnvironmentDomain string

@secure()
param orderingDbConnectionString string

resource containerApp 'Microsoft.App/containerApps@2022-03-01' = {
  name: 'ordering-api'
  location: location
  properties: {
    managedEnvironmentId: containerAppsEnvironmentId
    template: {
      containers: [
        {
          name: 'ordering-api'
          image: 'eshopdapr/ordering.api:latest'
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
              name: 'ConnectionStrings__OrderingDB'
              secretRef: 'orderingdb-connection-string'
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
        appId: 'ordering-api'
        appPort: 80
      }
      ingress: {
        external: false
        targetPort: 80
        allowInsecure: true
      }
      secrets: [
        {
          name: 'orderingdb-connection-string'
          value: orderingDbConnectionString
        }
      ]
    }
  }
}
