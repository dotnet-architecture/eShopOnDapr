param location string
param containerAppsEnvironmentId string
param containerAppsDomain string

resource containerApp 'Microsoft.Web/containerApps@2021-03-01' = {
  name: 'webshopping-gw'
  location: location
  properties: {
    kubeEnvironmentId: containerAppsEnvironmentId
    template: {
      containers: [
        {
          name: 'webshopping-gw'
          image: 'eshopdapr/webshoppingapigw:latest'
          env: [
            {
              name: 'ENVOY_CATALOG_API_ADDRESS'
              value: 'https://catalog-api.${containerAppsDomain}'
            }
            {
              name: 'ENVOY_ORDERING_API_ADDRESS'
              value: 'https://ordering-api.${containerAppsDomain}'
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
    }
  }
}

//output fqdn string = containerApp.properties.configuration.ingress.fqdn
