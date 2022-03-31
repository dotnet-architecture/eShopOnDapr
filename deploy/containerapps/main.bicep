param location string = resourceGroup().location
param uniqueSeed string = '${resourceGroup().id}-${deployment().name}'

////////////////////////////////////////////////////////////////////////////////
// Infrastructure
////////////////////////////////////////////////////////////////////////////////

module containerAppsEnvironment 'modules/infra/container-apps-env.bicep' = {
  name: '${deployment().name}-infra-container-app-env'
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
// Container apps
////////////////////////////////////////////////////////////////////////////////

module basketApi 'modules/apps/basket-api.bicep' = {
  name: '${deployment().name}-app-basket-api'
  dependsOn: [
    containerAppsEnvironment
    cosmos
    seq
    serviceBus
  ]
  params: {
    location: location
    seqFqdn: seq.outputs.fqdn
    containerAppsEnvironmentId: containerAppsEnvironment.outputs.containerAppsEnvironmentId
    containerAppsEnvironmentDomain: containerAppsEnvironment.outputs.containerAppsEnvironmentDomain
    cosmosDbName: cosmos.outputs.cosmosDbName
    cosmosCollectionName: cosmos.outputs.cosmosCollectionName
    cosmosUrl: cosmos.outputs.cosmosUrl
    cosmosKey: cosmos.outputs.cosmosKey
    serviceBusConnectionString: serviceBus.outputs.connectionString
  }
}

module blazorClient 'modules/apps/blazor-client.bicep' = {
  name: '${deployment().name}-app-blazor-client'
  dependsOn: [
    containerAppsEnvironment
    seq
  ]
  params: {
    location: location
    seqFqdn: seq.outputs.fqdn
    containerAppsEnvironmentId: containerAppsEnvironment.outputs.containerAppsEnvironmentId
    containerAppsEnvironmentDomain: containerAppsEnvironment.outputs.containerAppsEnvironmentDomain
  }
}

module catalogApi 'modules/apps/catalog-api.bicep' = {
  name: '${deployment().name}-app-catalog-api'
  dependsOn: [
    containerAppsEnvironment
    seq
    serviceBus
    sqlServer
  ]
  params: {
    location: location
    seqFqdn: seq.outputs.fqdn
    containerAppsEnvironmentId: containerAppsEnvironment.outputs.containerAppsEnvironmentId
    catalogDbConnectionString: sqlServer.outputs.catalogDbConnectionString
    serviceBusConnectionString: serviceBus.outputs.connectionString
  }
}

module identityApi 'modules/apps/identity-api.bicep' = {
  name: '${deployment().name}-app-identity-api'
  dependsOn: [
    containerAppsEnvironment
    seq
    sqlServer
  ]
  params: {
    location: location
    seqFqdn: seq.outputs.fqdn
    containerAppsEnvironmentId: containerAppsEnvironment.outputs.containerAppsEnvironmentId
    containerAppsEnvironmentDomain: containerAppsEnvironment.outputs.containerAppsEnvironmentDomain
    identityDbConnectionString: sqlServer.outputs.identityDbConnectionString
  }
}

module orderingApi 'modules/apps/ordering-api.bicep' = {
  name: '${deployment().name}-app-ordering-api'
  dependsOn: [
    containerAppsEnvironment
    cosmos
    seq
    serviceBus
    sqlServer
  ]
  params: {
    location: location
    seqFqdn: seq.outputs.fqdn
    containerAppsEnvironmentId: containerAppsEnvironment.outputs.containerAppsEnvironmentId
    containerAppsEnvironmentDomain: containerAppsEnvironment.outputs.containerAppsEnvironmentDomain
    cosmosDbName: cosmos.outputs.cosmosDbName
    cosmosCollectionName: cosmos.outputs.cosmosCollectionName
    cosmosUrl: cosmos.outputs.cosmosUrl
    cosmosKey: cosmos.outputs.cosmosKey
    orderingDbConnectionString: sqlServer.outputs.identityDbConnectionString
    serviceBusConnectionString: serviceBus.outputs.connectionString
  }
}

module paymentApi 'modules/apps/payment-api.bicep' = {
  name: '${deployment().name}-app-payment-api'
  dependsOn: [
    containerAppsEnvironment
    seq
    serviceBus
  ]
  params: {
    location: location
    seqFqdn: seq.outputs.fqdn
    containerAppsEnvironmentId: containerAppsEnvironment.outputs.containerAppsEnvironmentId
    serviceBusConnectionString: serviceBus.outputs.connectionString
  }
}

module seq 'modules/apps/seq.bicep' = {
  name: '${deployment().name}-app-seq'
  dependsOn: [
    containerAppsEnvironment
  ]
  params: {
    location: location
    containerAppsEnvironmentId: containerAppsEnvironment.outputs.containerAppsEnvironmentId
  }
}

module webshoppingAgg 'modules/apps/webshopping-agg.bicep' = {
  name: '${deployment().name}-app-webshopping-agg'
  dependsOn: [
    containerAppsEnvironment
    seq
  ]
  params: {
    location: location
    containerAppsEnvironmentId: containerAppsEnvironment.outputs.containerAppsEnvironmentId
    containerAppsEnvironmentDomain: containerAppsEnvironment.outputs.containerAppsEnvironmentDomain
    seqFqdn: seq.outputs.fqdn
  }
}

module webshoppingGW 'modules/apps/webshopping-gw.bicep' = {
  name: '${deployment().name}-app-webshopping-gw'
  dependsOn: [
    containerAppsEnvironment
    seq
  ]
  params: {
    location: location
    containerAppsEnvironmentId: containerAppsEnvironment.outputs.containerAppsEnvironmentId
    containerAppsEnvironmentDomain: containerAppsEnvironment.outputs.containerAppsEnvironmentDomain
  }
}

module webstatus 'modules/apps/webstatus.bicep' = {
  name: '${deployment().name}-app-webstatus'
  dependsOn: [
    containerAppsEnvironment
    seq
  ]
  params: {
    location: location
    containerAppsEnvironmentId: containerAppsEnvironment.outputs.containerAppsEnvironmentId
    containerAppsEnvironmentDomain: containerAppsEnvironment.outputs.containerAppsEnvironmentDomain
  }
}
