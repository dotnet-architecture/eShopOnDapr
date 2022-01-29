param location string
param containerAppsEnvironmentId string

resource containerApp 'Microsoft.Web/containerApps@2021-03-01' = {
  name: 'webshopping-gw'
  location: location
  properties: {
    kubeEnvironmentId: containerAppsEnvironmentId
    template: {
      containers: [
        {
          name: 'webshopping-gw'
          image: 'envoyproxy/envoy:v1.14.2'
          env: [
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
