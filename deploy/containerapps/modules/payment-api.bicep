param location string
param containerAppsEnvironmentId string
param seqFqdn string

resource containerApp 'Microsoft.Web/containerApps@2021-03-01' = {
  name: 'payment-api'
  location: location
  properties: {
    kubeEnvironmentId: containerAppsEnvironmentId
    template: {
      containers: [
        {
          name: 'payment-api'
          image: 'eshopdapr/payment.api:latest'
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
              name: 'SeqServerUrl'
              value: 'https://${seqFqdn}'
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
