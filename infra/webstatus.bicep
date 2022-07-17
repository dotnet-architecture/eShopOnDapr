param location string
@minLength(1)
@maxLength(64)
@description('Name of the the environment which is used to generate a short unqiue hash used in all resources.')
param name string
param imageName string

var resourceToken = toLower(uniqueString(subscription().id, name, location))
var tags = {
  'azd-env-name': name
}

resource containerAppsEnvironment 'Microsoft.App/managedEnvironments@2022-01-01-preview' existing = {
  name: 'cae-${resourceToken}'
}

resource containerRegistry 'Microsoft.ContainerRegistry/registries@2021-12-01-preview' existing = {
  name: 'contreg${resourceToken}'
}

resource appInsights 'Microsoft.Insights/components@2020-02-02' existing = {
  name: 'appi-${resourceToken}'
}

resource keyVault 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
  name: 'keyvault${resourceToken}'
}

resource webstatus 'Microsoft.App/containerApps@2022-03-01' = {
  name: 'ca-webstatus-${resourceToken}'
  location: location
  tags: union(tags, {
      'azd-service-name': 'webstatus'
    })
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    managedEnvironmentId: containerAppsEnvironment.id
    template: {
      containers: [
        {
          name: 'main'
          image: imageName//'eshopdapr/webstatus:latest'
          env: [
            {
              name: 'ASPNETCORE_URLS'
              value: 'http://0.0.0.0:80'
            }
            {
              name: 'HealthChecksUI__HealthChecks__0__Name'
              value: 'Basket API'
            }  
            {
              name: 'HealthChecksUI__HealthChecks__0__Uri'
              value: 'http://basket-api.internal.${containerAppsEnvironment.properties.defaultDomain}/hc'
            }
            {
              name: 'HealthChecksUI__HealthChecks__1__Name'
              value: 'Catalog API'
            }  
            {
              name: 'HealthChecksUI__HealthChecks__1__Uri'
              value: 'http://catalog-api.internal.${containerAppsEnvironment.properties.defaultDomain}/hc'
            }
            {
              name: 'HealthChecksUI__HealthChecks__2__Name'
              value: 'Identity API'
            }  
            {
              name: 'HealthChecksUI__HealthChecks__2__Uri'
              value: 'https://identity-api.${containerAppsEnvironment.properties.defaultDomain}/hc'
            }
            {
              name: 'HealthChecksUI__HealthChecks__3__Name'
              value: 'Ordering API'
            }  
            {
              name: 'HealthChecksUI__HealthChecks__3__Uri'
              value: 'http://ordering-api.internal.${containerAppsEnvironment.properties.defaultDomain}/hc'
            }
            {
              name: 'HealthChecksUI__HealthChecks__4__Name'
              value: 'Payment API'
            }  
            {
              name: 'HealthChecksUI__HealthChecks__4__Uri'
              value: 'http://payment-api.internal.${containerAppsEnvironment.properties.defaultDomain}/hc'
            }
            {
              name: 'HealthChecksUI__HealthChecks__5__Name'
              value: 'Web Shopping Aggregator'
            }  
            {
              name: 'HealthChecksUI__HealthChecks__5__Uri'
              value: 'http://webshopping-agg.internal.${containerAppsEnvironment.properties.defaultDomain}/hc'
            }
            {
              name: 'HealthChecksUI__HealthChecks__6__Name'
              value: 'Blazor UI Host'
            }  
            {
              name: 'HealthChecksUI__HealthChecks__6__Uri'
              value: 'https://blazor-client.${containerAppsEnvironment.properties.defaultDomain}/hc'
            }
            {
              name: 'WebStatus_APP_APPINSIGHTS_INSTRUMENTATIONKEY'
              value: appInsights.properties.InstrumentationKey
            }
            {
              name: 'AZURE_KEY_VAULT_ENDPOINT'
              value: keyVault.properties.vaultUri
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
      secrets: [
        {
          name: 'registry-password'
          value: containerRegistry.listCredentials().passwords[0].value
        }
      ]
      registries: [
        {
          server: '${containerRegistry.name}.azurecr.io'
          username: containerRegistry.name
          passwordSecretRef: 'registry-password'
        }
      ]
    }
  }
}


resource keyVaultAccessPolicies 'Microsoft.KeyVault/vaults/accessPolicies@2021-11-01-preview' = {
  name: '${keyVault.name}/add'
  properties: {
    accessPolicies: [
      {
        objectId: webstatus.identity.principalId
        permissions: {
          secrets: [
            'get'
            'list'
          ]
        }
        tenantId: subscription().tenantId
      }
    ]
  }
}

output WEB_URI string = 'https://${webstatus.properties.configuration.ingress.fqdn}'
