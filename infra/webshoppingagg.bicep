param location string
param seqFqdn string
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

resource webshoppingagg 'Microsoft.App/containerApps@2022-03-01' = {
  name: 'ca-webshoppingagg-${resourceToken}'
  location: location
  tags: union(tags, {
    'azd-service-name': 'webshoppingagg'
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
          image: imageName//'eshopdapr/webshoppingagg:20220331'
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
              value: 'https://identity-api.${containerAppsEnvironment.properties.defaultDomain}'
            }  
            {
              name: 'IdentityUrlExternal'
              value: 'https://identity-api.${containerAppsEnvironment.properties.defaultDomain}'
            }
            {
              name: 'SeqServerUrl'
              value: 'https://${seqFqdn}'
            }
            {
              name: 'BasketUrlHC'
              value: 'http://basket-api.internal.${containerAppsEnvironment.properties.defaultDomain}/hc'
            }
            {
              name: 'CatalogUrlHC'
              value: 'http://catalog-api.internal.${containerAppsEnvironment.properties.defaultDomain}/hc'
            }
            {
              name: 'IdentityUrlHC'
              value: 'https://identity-api.${containerAppsEnvironment.properties.defaultDomain}/hc'
            }
            {
              name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
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
      dapr: {
        enabled: true
        appId: 'webshoppingagg'
        appPort: 80
      }
      ingress: {
        external: false
        targetPort: 80
        allowInsecure: true
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
        objectId: webshoppingagg.identity.principalId
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

output API_URI string = 'https://${webshoppingagg.properties.configuration.ingress.fqdn}'
