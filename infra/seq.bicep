param location string
@minLength(1)
@maxLength(64)
@description('Name of the the environment which is used to generate a short unqiue hash used in all resources.')
param name string

var resourceToken = toLower(uniqueString(subscription().id, name, location))
var tags = {
  'azd-env-name': name
}

resource containerAppsEnvironment 'Microsoft.App/managedEnvironments@2022-01-01-preview' existing = {
  name: 'cae-${resourceToken}'
}

resource appInsights 'Microsoft.Insights/components@2020-02-02' existing = {
  name: 'appi-${resourceToken}'
}

param containerAppsEnvironmentId string

resource seq 'Microsoft.App/containerApps@2022-03-01' = {
  name: 'seq'
  location: location
  tags: union(tags, {
    'azd-service-name': 'seq'
    })
  properties: {
    managedEnvironmentId: containerAppsEnvironmentId
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
      activeRevisionsMode: 'single'
      ingress: {
        external: true
        targetPort: 80
      }
    }
  }
}

output fqdn string = seq.properties.configuration.ingress.fqdn
