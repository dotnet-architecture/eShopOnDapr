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

param uniqueSeed string = '${resourceGroup().id}-${deployment().name}'

resource logAnalyticsWorkspace 'Microsoft.OperationalInsights/workspaces@2021-06-01' = {
  name: 'log-${resourceToken}'
  location: location
  tags: tags
  properties: any({
    retentionInDays: 30
    features: {
      searchVersion: 1
    }
    sku: {
      name: 'PerGB2018'
    }
  })
}


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


resource containerAppsEnvironment 'Microsoft.App/managedEnvironments@2022-03-01' = {
  name: 'cae-${resourceToken}'
  location: location
  tags: tags
  properties: {
    appLogsConfiguration: {
      destination: 'log-analytics'
      logAnalyticsConfiguration: {
        customerId: logAnalyticsWorkspace.properties.customerId
        sharedKey: logAnalyticsWorkspace.listKeys().primarySharedKey
      }
    }
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
    containerAppsEnvironmentName: 'cae-${resourceToken}'
    serviceBusConnectionString: serviceBus.outputs.connectionString
  }
}

module daprStateStore 'modules/dapr/statestore.bicep' = {
  name: '${deployment().name}-dapr-statestore'
  params: {
    containerAppsEnvironmentName: 'cae-${resourceToken}'
    cosmosDbName: cosmos.outputs.cosmosDbName
    cosmosCollectionName: cosmos.outputs.cosmosCollectionName
    cosmosUrl: cosmos.outputs.cosmosUrl
    cosmosKey: cosmos.outputs.cosmosKey
  }
}

////////////////////////////////////////////////////////////////////////////////
// Container apps
////////////////////////////////////////////////////////////////////////////////




module basketapi './basketapi.bicep' = {
  name: '${deployment().name}-app-basketapi'
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
    imageName: basketapiImageName != '' ? basketapiImageName : 'nginx:latest'
  }
}
module blazorClient './blazorclient.bicep' = {
  name: '${deployment().name}-app-blazorclient'
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
    imageName: blazorclientImageName != '' ? blazorclientImageName : 'nginx:latest'
  }
}
module catalogapi './catalogapi.bicep' = {
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
    catalogDbConnectionString: sqlServer.outputs.catalogDbConnectionString
    imageName: catalogapiImageName != '' ? catalogapiImageName : 'nginx:latest'
  }
}


module identityapi './identityapi.bicep' = {
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
    identityDbConnectionString: sqlServer.outputs.identityDbConnectionString
    imageName: identityapiImageName != '' ? identityapiImageName : 'nginx:latest'
  }
}

module orderingapi './orderingapi.bicep' = {
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
    orderingDbConnectionString: sqlServer.outputs.identityDbConnectionString
    imageName: orderingapiImageName != '' ? orderingapiImageName : 'nginx:latest'
  }
}

module paymentapi './paymentapi.bicep' = {
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
    imageName: paymentapiImageName != '' ? paymentapiImageName : 'nginx:latest'
  }
}

module seq './seq.bicep' = {
  name: '${deployment().name}-app-seq'
  params: {
    name:name
    location: location
    containerAppsEnvironmentId: containerAppsEnvironment.id
  }
}

module webshoppingAgg './webshoppingagg.bicep' = {
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
    seqFqdn: seq.outputs.fqdn
    imageName: webshoppingaggImageName != '' ? webshoppingaggImageName : 'nginx:latest'
  }
}

module webshoppinggw './webshoppinggw.bicep' = {
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
    imageName: webshoppinggwImageName != '' ? webshoppinggwImageName : 'nginx:latest'
  }
}

module webstatus './webstatus.bicep' = {
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
    imageName: webstatusImageName != '' ? webstatusImageName : 'nginx:latest'
  }
}



output AZURE_COSMOS_CONNECTION_STRING_KEY string = 'AZURE-COSMOS-CONNECTION-STRING'
output AZURE_COSMOS_DATABASE_NAME string = cosmos.outputs.cosmosDbName
output AZURE_KEY_VAULT_ENDPOINT string = keyVault.properties.vaultUri
output APPINSIGHTS_INSTRUMENTATIONKEY string = appInsightsResources.outputs.APPINSIGHTS_INSTRUMENTATIONKEY
output AZURE_CONTAINER_REGISTRY_ENDPOINT string = containerRegistry.properties.loginServer
output AZURE_CONTAINER_REGISTRY_NAME string = containerRegistry.name
//output WEB_URI string = webstatus.outputs.WEB_URI
output BASKETAPI_URI string = basketapi.outputs.API_URI
