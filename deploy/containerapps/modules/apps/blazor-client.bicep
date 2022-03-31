param location string
param seqFqdn string

param containerAppsEnvironmentId string
param containerAppsEnvironmentDomain string

resource containerApp 'Microsoft.Web/containerApps@2021-03-01' = {
  name: 'blazor-client'
  location: location
  properties: {
    kubeEnvironmentId: containerAppsEnvironmentId
    template: {
      containers: [
        {
          name: 'blazor-client'
          image: 'eshopdapr/blazor.client:20220331'
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
              name: 'ApiGatewayUrlExternal'
              value: 'https://webshopping-gw.${containerAppsEnvironmentDomain}'
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
    }
    configuration: {
      activeResivionsMode: 'single'
      ingress: {
        external: true
        targetPort: 80
      }
    }
  }
}
