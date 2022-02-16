param location string
param containerAppsEnvironmentId string

resource containerApp 'Microsoft.Web/containerApps@2021-03-01' = {
  name: 'seq'
  location: location
  properties: {
    kubeEnvironmentId: containerAppsEnvironmentId
    template: {
      containers: [
        {
          name: 'seq'
          image: 'datalust/seq:latest'
          env: [
            {
              name: 'ACCEPT_EULA'
              value: 'Y'
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

output fqdn string = containerApp.properties.configuration.ingress.fqdn
