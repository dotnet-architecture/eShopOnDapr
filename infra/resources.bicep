param name string
param location string
param principalId string = ''
param resourceToken string
param tags object
param basketapiImageName string = ''
param blazorclientImageName string = ''
param catalogapiImageName string = ''
param identityapiImageName string = ''
param orderingapiImageName string = ''
param paymentapiImageName string = ''
param webshoppingaggImageName string = ''
param webshoppinggwImageName string = ''
param webstatusImageName string = ''

param uniqueSeed string = name



resource containerRegistry 'Microsoft.ContainerRegistry/registries@2021-12-01-preview' = {
  name: 'contreg${resourceToken}'
  location: location
  tags: tags
  sku: {
    name: 'Standard'
  }
  properties: {
    adminUserEnabled: true
    anonymousPullEnabled: false
    dataEndpointEnabled: false
    encryption: {
      status: 'disabled'
    }
    networkRuleBypassOptions: 'AzureServices'
    publicNetworkAccess: 'Enabled'
    zoneRedundancy: 'Disabled'
  }
}

resource keyVault 'Microsoft.KeyVault/vaults@2019-09-01' = {
  name: 'keyvault${resourceToken}'
  location: location
  tags: tags
  properties: {
    tenantId: subscription().tenantId
    sku: {
      family: 'A'
      name: 'standard'
    }
    accessPolicies: []
  }

  resource cosmosconnectionstring 'secrets' = {
    name: 'AZURE-COSMOS-CONNECTION-STRING'
    properties: {
      value: cosmos.outputs.cosmosConnectionString
    }
  }
}

resource keyVaultAccessPolicies 'Microsoft.KeyVault/vaults/accessPolicies@2021-11-01-preview' = if (!empty(principalId)) {
  name: '${keyVault.name}/add'
  properties: {
    accessPolicies: [
      {
        objectId: principalId
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

module appInsightsResources './appinsights.bicep' = {
  name: 'appinsights-${resourceToken}'
  params: {
    resourceToken: resourceToken
    location: location
    tags: tags
  }
}

////////////////////////////////////////////////////////////////////////////////
// Infrastructure
////////////////////////////////////////////////////////////////////////////////

module containerAppsEnvironment 'modules/infra/container-apps-env.bicep' = {
  name: 'cae-${resourceToken}'
  params: {
    location: location
    uniqueSeed: uniqueSeed
  }
}

module cosmos 'modules/infra/cosmos-db.bicep' = {
  name: '${deployment().name}-infra-cosmos-db'
  params: {
    location: location
    uniqueSeed: uniqueSeed
  }
}

module serviceBus 'modules/infra/service-bus.bicep' = {
  name: '${deployment().name}-infra-service-bus'
  params: {
    location: location
    uniqueSeed: uniqueSeed
  }
}

module sqlServer 'modules/infra/sql-server.bicep' = {
  name: '${deployment().name}-infra-sql-server'
  params: {
    location: location
    uniqueSeed: uniqueSeed
  }
}

////////////////////////////////////////////////////////////////////////////////
// Dapr components
////////////////////////////////////////////////////////////////////////////////

module daprPubSub 'modules/dapr/pubsub.bicep' = {
  name: '${deployment().name}-dapr-pubsub'
  params: {
    containerAppsEnvironmentName: containerAppsEnvironment.outputs.name
    serviceBusConnectionString: serviceBus.outputs.connectionString
  }
}

module daprStateStore 'modules/dapr/statestore.bicep' = {
  name: '${deployment().name}-dapr-statestore'
  params: {
    containerAppsEnvironmentName: containerAppsEnvironment.outputs.name
    cosmosDbName: cosmos.outputs.cosmosDbName
    cosmosCollectionName: cosmos.outputs.cosmosCollectionName
    cosmosUrl: cosmos.outputs.cosmosUrl
    cosmosKey: cosmos.outputs.cosmosKey
  }
}

////////////////////////////////////////////////////////////////////////////////
// Container apps
////////////////////////////////////////////////////////////////////////////////


module seq 'modules/apps/seq.bicep' = {
  name: '${deployment().name}-app-seq'
  params: {
    name:name
    location: location
    containerAppsEnvironmentId: containerAppsEnvironment.outputs.id
  }
}

module basketApi 'modules/apps/basketapi.bicep' = {
  name: '${deployment().name}-app-basket-api'
  dependsOn: [
    containerAppsEnvironment
    containerRegistry
    appInsightsResources
    keyVault
    daprPubSub
    daprStateStore
    seq
  ]
  params: {
    name:name
    location: location
    seqFqdn: seq.outputs.fqdn
    containerAppsEnvironmentId: containerAppsEnvironment.outputs.id
    containerAppsEnvironmentDomain: containerAppsEnvironment.outputs.domain
    imageName: basketapiImageName != '' ? basketapiImageName : 'nginx:latest'
  }
}
module blazorClient 'modules/apps/blazorclient.bicep' = {
  name: '${deployment().name}-app-blazor-client'
  dependsOn: [
    containerAppsEnvironment
    containerRegistry
    appInsightsResources
    keyVault
    seq
  ]
  params: {
    name:name
    location: location
    seqFqdn: seq.outputs.fqdn
    containerAppsEnvironmentId: containerAppsEnvironment.outputs.id
    containerAppsEnvironmentDomain: containerAppsEnvironment.outputs.domain
    imageName: blazorclientImageName != '' ? blazorclientImageName : 'nginx:latest'
  }
}
module catalogApi 'modules/apps/catalogapi.bicep' = {
  name: '${deployment().name}-app-catalog-api'
  dependsOn: [
    containerAppsEnvironment
    containerRegistry
    appInsightsResources
    keyVault
    daprPubSub
    seq
  ]
  params: {
    name:name
    location: location
    seqFqdn: seq.outputs.fqdn
    containerAppsEnvironmentId: containerAppsEnvironment.outputs.id
    catalogDbConnectionString: sqlServer.outputs.catalogDbConnectionString
    imageName: catalogapiImageName != '' ? catalogapiImageName : 'nginx:latest'
  }
}


module identityApi 'modules/apps/identityapi.bicep' = {
  name: '${deployment().name}-app-identity-api'
  dependsOn: [
    containerAppsEnvironment
    containerRegistry
    appInsightsResources
    keyVault
    seq
  ]
  params: {
    name:name
    location: location
    seqFqdn: seq.outputs.fqdn
    containerAppsEnvironmentId: containerAppsEnvironment.outputs.id
    containerAppsEnvironmentDomain: containerAppsEnvironment.outputs.domain
    identityDbConnectionString: sqlServer.outputs.identityDbConnectionString
    imageName: identityapiImageName != '' ? identityapiImageName : 'nginx:latest'
  }
}

module orderingApi 'modules/apps/orderingapi.bicep' = {
  name: '${deployment().name}-app-ordering-api'
  dependsOn: [
    containerAppsEnvironment
    containerRegistry
    appInsightsResources
    keyVault
    daprPubSub
    daprStateStore
    seq
  ]
  params: {
    name:name
    location: location
    seqFqdn: seq.outputs.fqdn
    containerAppsEnvironmentId: containerAppsEnvironment.outputs.id
    containerAppsEnvironmentDomain: containerAppsEnvironment.outputs.domain
    orderingDbConnectionString: sqlServer.outputs.identityDbConnectionString
    imageName: orderingapiImageName != '' ? orderingapiImageName : 'nginx:latest'
  }
}

module paymentApi 'modules/apps/paymentapi.bicep' = {
  name: '${deployment().name}-app-payment-api'
  dependsOn: [
    containerAppsEnvironment
    containerRegistry
    appInsightsResources
    keyVault
    daprPubSub
    seq
  ]
  params: {
    name:name
    location: location
    seqFqdn: seq.outputs.fqdn
    containerAppsEnvironmentId: containerAppsEnvironment.outputs.id
    imageName: paymentapiImageName != '' ? paymentapiImageName : 'nginx:latest'
  }
}

module webshoppingAgg 'modules/apps/webshoppingagg.bicep' = {
  name: '${deployment().name}-app-webshopping-agg'
  dependsOn: [
    containerAppsEnvironment
    containerRegistry
    appInsightsResources
    keyVault
    seq
  ]
  params: {
    name:name
    location: location
    containerAppsEnvironmentId: containerAppsEnvironment.outputs.id
    containerAppsEnvironmentDomain: containerAppsEnvironment.outputs.domain
    seqFqdn: seq.outputs.fqdn
    imageName: webshoppingaggImageName != '' ? webshoppingaggImageName : 'nginx:latest'
  }
}

module webshoppingGW 'modules/apps/webshoppinggw.bicep' = {
  name: '${deployment().name}-app-webshopping-gw'
  dependsOn: [
    containerAppsEnvironment
    containerRegistry
    appInsightsResources
    keyVault
    seq
  ]
  params: {
    name:name
    location: location
    containerAppsEnvironmentId: containerAppsEnvironment.outputs.id
    containerAppsEnvironmentDomain: containerAppsEnvironment.outputs.domain
    imageName: webshoppinggwImageName != '' ? webshoppinggwImageName : 'nginx:latest'
  }
}

module webstatus 'modules/apps/webstatus.bicep' = {
  name: '${deployment().name}-app-webstatus'
  dependsOn: [
    containerAppsEnvironment
    containerRegistry
    appInsightsResources
    keyVault
    seq
  ]
  params: {
    name:name
    location: location
    containerAppsEnvironmentId: containerAppsEnvironment.outputs.id
    containerAppsEnvironmentDomain: containerAppsEnvironment.outputs.domain
    imageName: webstatusImageName != '' ? webstatusImageName : 'nginx:latest'
  }
}



output AZURE_COSMOS_CONNECTION_STRING_KEY string = 'AZURE-COSMOS-CONNECTION-STRING'
output AZURE_COSMOS_DATABASE_NAME string = cosmos.outputs.cosmosDbName
output AZURE_KEY_VAULT_ENDPOINT string = keyVault.properties.vaultUri
output APPINSIGHTS_INSTRUMENTATIONKEY string = appInsightsResources.outputs.APPINSIGHTS_INSTRUMENTATIONKEY
output AZURE_CONTAINER_REGISTRY_ENDPOINT string = containerRegistry.properties.loginServer
output AZURE_CONTAINER_REGISTRY_NAME string = containerRegistry.name
output WEB_URI string = webstatus.outputs.WEB_URI
output API_URI string = basketApi.outputs.API_URI
