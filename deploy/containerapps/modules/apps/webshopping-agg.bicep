param location string
param seqFqdn string

param containerAppsEnvironmentId string
param containerAppsEnvironmentDomain string

resource containerApp 'Microsoft.Web/containerApps@2021-03-01' = {
  name: 'webshopping-agg'
  location: location
  properties: {
    kubeEnvironmentId: containerAppsEnvironmentId
    template: {
      containers: [
        {
          name: 'webshopping-agg'
          image: 'eshopdapr/webshoppingagg:20220331'
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
            {
              name: 'BasketUrlHC'
              value: 'http://basket-api.internal.${containerAppsEnvironmentDomain}/hc'
            }
            {
              name: 'CatalogUrlHC'
              value: 'http://catalog-api.internal.${containerAppsEnvironmentDomain}/hc'
            }
            {
              name: 'IdentityUrlHC'
              value: 'https://identity-api.${containerAppsEnvironmentDomain}/hc'
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
        appId: 'webshoppingagg'
        appPort: 80
      }
    }
    configuration: {
      activeResivionsMode: 'single'
      ingress: {
        external: false
        targetPort: 80
        allowInsecure: true
      }
    }
  }
}
