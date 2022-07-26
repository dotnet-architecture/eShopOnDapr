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


param sqlServerName string = 'sql-${uniqueString(uniqueSeed)}'
param sqlAdministratorLogin string = 'server_admin'
param sqlAdministratorLoginPassword string = 'Pass@word'
param catalogDbName string = 'Microsoft.eShopOnDapr.Services.CatalogDb'
param identityDbName string = 'Microsoft.eShopOnDapr.Services.IdentityDb'
param orderingDbName string = 'Microsoft.eShopOnDapr.Services.OrderingDb'

////////////////////////////////////////////////////////////////////////////////
// Infrastructure
////////////////////////////////////////////////////////////////////////////////

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

resource serviceBus 'Microsoft.ServiceBus/namespaces@2021-06-01-preview' = {
  name: 'sb-${resourceToken}'
  location: location
  sku: {
    name: 'Standard'
    tier: 'Standard'
  }
}

resource sqlServer 'Microsoft.Sql/servers@2021-05-01-preview' = {
  name: sqlServerName
  location: location
  properties: {
    administratorLogin: sqlAdministratorLogin
    administratorLoginPassword: sqlAdministratorLoginPassword
  }

  resource sqlServerFirewall 'firewallRules@2021-05-01-preview' = {
    name: 'AllowAllWindowsAzureIps'
    properties: {
      // Allow Azure services and resources to access this server
      startIpAddress: '0.0.0.0'
      endIpAddress: '0.0.0.0'
    }
  }

  resource catalogDB 'databases@2021-05-01-preview' = {
    name: 'Microsoft.eShopOnDapr.Services.CatalogDb'
    location: location
    properties: {
      collation: 'SQL_Latin1_General_CP1_CI_AS'
    }
  }

  resource identityDb 'databases@2021-05-01-preview' = {
    name: 'Microsoft.eShopOnDapr.Services.IdentityDb'
    location: location
    properties: {
      collation: 'SQL_Latin1_General_CP1_CI_AS'
    }
  }

  resource orderingDb 'databases@2021-05-01-preview' = {
    name: 'Microsoft.eShopOnDapr.Services.OrderingDb'
    location: location
    properties: {
      collation: 'SQL_Latin1_General_CP1_CI_AS'
    }
  }
}

////////////////////////////////////////////////////////////////////////////////
// Dapr components
////////////////////////////////////////////////////////////////////////////////

module daprPubSub 'modules/dapr/pubsub.bicep' = {
  name: '${deployment().name}-dapr-pubsub'
  dependsOn:[
    containerAppsEnvironment
  ]
  params: {
    containerAppsEnvironmentName: 'cae-${resourceToken}'
    serviceBusConnectionString: 'Endpoint=sb://${serviceBus.name}.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=${listKeys('${serviceBus.id}/AuthorizationRules/RootManageSharedAccessKey', serviceBus.apiVersion).primaryKey}'
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

module blazorclient './blazorclient.bicep' = {
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
    catalogDbConnectionString: 'Server=tcp:${sqlServerName}${environment().suffixes.sqlServerHostname},1433;Initial Catalog=${catalogDbName};Persist Security Info=False;User ID=${sqlAdministratorLogin};Password=${sqlAdministratorLoginPassword};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'
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
    identityDbConnectionString: 'Server=tcp:${sqlServerName}${environment().suffixes.sqlServerHostname},1433;Initial Catalog=${identityDbName};Persist Security Info=False;User ID=${sqlAdministratorLogin};Password=${sqlAdministratorLoginPassword};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'
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
    orderingDbConnectionString: 'Server=tcp:${sqlServerName}${environment().suffixes.sqlServerHostname},1433;Initial Catalog=${orderingDbName};Persist Security Info=False;User ID=${sqlAdministratorLogin};Password=${sqlAdministratorLoginPassword};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'
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
output WEBSTATUS_URI string = webstatus.outputs.WEBSTATUS_URI
output WEB_BLAZORCLIENT string = blazorclient.outputs.BLAZORCLIENT_URI
output SEQ_FQDN string = seq.outputs.fqdn
output CATALOG_DB_CONN_STRING string = 'Server=tcp:${sqlServerName}${environment().suffixes.sqlServerHostname},1433;Initial Catalog=${catalogDbName};Persist Security Info=False;User ID=${sqlAdministratorLogin};Password=${sqlAdministratorLoginPassword};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'
output IDENTITY_DB_CONN_STRING string = 'Server=tcp:${sqlServerName}${environment().suffixes.sqlServerHostname},1433;Initial Catalog=${identityDbName};Persist Security Info=False;User ID=${sqlAdministratorLogin};Password=${sqlAdministratorLoginPassword};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'
output ORDERING_DB_CONN_STRING string = 'Server=tcp:${sqlServerName}${environment().suffixes.sqlServerHostname},1433;Initial Catalog=${orderingDbName};Persist Security Info=False;User ID=${sqlAdministratorLogin};Password=${sqlAdministratorLoginPassword};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'
