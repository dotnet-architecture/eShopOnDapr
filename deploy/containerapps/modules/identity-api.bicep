param location string
param containerAppsEnvironmentId string
param containerAppsDomain string
param identityDbConnectionString string
param seqFqdn string

resource containerApp 'Microsoft.Web/containerApps@2021-03-01' = {
  name: 'identity-api'
  location: location
  properties: {
    kubeEnvironmentId: containerAppsEnvironmentId
    template: {
      containers: [
        {
          name: 'identity-api'
          image: 'eshopdapr/identity.api:latest'
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
              value: 'https://identity-api.${containerAppsDomain}'
            }
            {
              name: 'IdentityUrlExternal'
              value: 'https://identity-api.${containerAppsDomain}'
            }
            {
              name: 'SeqServerUrl'
              value: 'https://${seqFqdn}'
            }
            {
              name: 'BlazorClientUrlExternal'
              value: 'https://blazor-client.${containerAppsDomain}'
            }
            {
              name: 'BasketApiUrlExternal'
              value: 'https://basket-api.${containerAppsDomain}'
            }
            {
              name: 'OrderingApiUrlExternal'
              value: 'https://ordering-api.${containerAppsDomain}'
            }
            {
              name: 'ShoppingAggregatorApiUrlExternal'
              value: 'https://webshoppingagg.${containerAppsDomain}'
            }
            {
              name: 'ConnectionStrings__IdentityDB'
              secretRef: 'identitydb-connection-string'
            }
          ]
        }
      ]
      scale: {
        minReplicas: 1
      }
    }
    configuration: {
      activeResivionsMode: 'single'
      ingress: {
        external: true
        targetPort: 80
      }
      secrets: [
        {
          name: 'identitydb-connection-string'
          value: identityDbConnectionString
        }
      ]
    }
  }
}

output fqdn string = containerApp.properties.configuration.ingress.fqdn
