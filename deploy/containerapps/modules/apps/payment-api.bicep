param location string
param seqFqdn string

param containerAppsEnvironmentId string

@secure()
param serviceBusConnectionString string

resource containerApp 'Microsoft.Web/containerApps@2021-03-01' = {
  name: 'payment-api'
  location: location
  properties: {
    kubeEnvironmentId: containerAppsEnvironmentId
    template: {
      containers: [
        {
          name: 'payment-api'
          image: 'eshopdapr/payment.api:20220331'
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
        maxReplicas: 1
      }
      dapr: {
        enabled: true
        appId: 'payment-api'
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
              'payment-api'
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
          name: 'service-bus-connection-string'
          value: serviceBusConnectionString
        }
      ]
    }
  }
}
