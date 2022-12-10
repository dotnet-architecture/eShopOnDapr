param location string = resourceGroup().location
param uniqueSeed string = '${resourceGroup().id}-${deployment().name}'

////////////////////////////////////////////////////////////////////////////////
// Infrastructure
////////////////////////////////////////////////////////////////////////////////

module managedIdentity 'modules/infra/identity.bicep' = {
  name: '${deployment().name}-infra-managed-identity'
  params: {
    location: location
    uniqueSeed: uniqueSeed
  }
}

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

module keyVault 'modules/infra/keyvault.bicep' = {
  name: '${deployment().name}-infra-keyvault'
  params: {
    location: location
    uniqueSeed: uniqueSeed
    managedIdentityObjectId: managedIdentity.outputs.identityObjectId
    catalogDbConnectionString: sqlServer.outputs.catalogDbConnectionString
    identityDbConnectionString: sqlServer.outputs.identityDbConnectionString
    orderingDbConnectionString: sqlServer.outputs.orderingDbConnectionString
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

module daprSecretStore 'modules/dapr/secretstore.bicep' = {
  name: '${deployment().name}-dapr-secretstore'
  params: {
    containerAppsEnvironmentName: containerAppsEnvironment.outputs.name
    vaultName: keyVault.outputs.vaultName
    managedIdentityClientId: managedIdentity.outputs.identityClientId
  }
}

////////////////////////////////////////////////////////////////////////////////
// Container apps
////////////////////////////////////////////////////////////////////////////////

module basketApi 'modules/apps/basket-api.bicep' = {
  name: '${deployment().name}-app-basket-api'
  dependsOn: [
    daprPubSub
    daprStateStore
    seq
  ]
  params: {
    location: location
    seqFqdn: seq.outputs.fqdn
    containerAppsEnvironmentId: containerAppsEnvironment.outputs.id
    containerAppsEnvironmentDomain: containerAppsEnvironment.outputs.domain
  }
}

module blazorClient 'modules/apps/blazor-client.bicep' = {
  name: '${deployment().name}-app-blazor-client'
  dependsOn: [
    seq
  ]
  params: {
    location: location
    seqFqdn: seq.outputs.fqdn
    containerAppsEnvironmentId: containerAppsEnvironment.outputs.id
    containerAppsEnvironmentDomain: containerAppsEnvironment.outputs.domain
  }
}

module catalogApi 'modules/apps/catalog-api.bicep' = {
  name: '${deployment().name}-app-catalog-api'
  dependsOn: [
    daprPubSub
    daprSecretStore
    seq
  ]
  params: {
    location: location
    seqFqdn: seq.outputs.fqdn
    containerAppsEnvironmentId: containerAppsEnvironment.outputs.id
    managedIdentityId: managedIdentity.outputs.identityId
    catalogDbConnectionString: sqlServer.outputs.catalogDbConnectionString
  }
}

module identityApi 'modules/apps/identity-api.bicep' = {
  name: '${deployment().name}-app-identity-api'
  dependsOn: [
    daprSecretStore
    seq
  ]
  params: {
    location: location
    seqFqdn: seq.outputs.fqdn
    containerAppsEnvironmentId: containerAppsEnvironment.outputs.id
    containerAppsEnvironmentDomain: containerAppsEnvironment.outputs.domain
    managedIdentityId: managedIdentity.outputs.identityId
    identityDbConnectionString: sqlServer.outputs.identityDbConnectionString
  }
}

module orderingApi 'modules/apps/ordering-api.bicep' = {
  name: '${deployment().name}-app-ordering-api'
  dependsOn: [
    daprPubSub
    daprStateStore
    daprSecretStore
    seq
  ]
  params: {
    location: location
    seqFqdn: seq.outputs.fqdn
    containerAppsEnvironmentId: containerAppsEnvironment.outputs.id
    containerAppsEnvironmentDomain: containerAppsEnvironment.outputs.domain
    managedIdentityId: managedIdentity.outputs.identityId
    orderingDbConnectionString: sqlServer.outputs.identityDbConnectionString
  }
}

module paymentApi 'modules/apps/payment-api.bicep' = {
  name: '${deployment().name}-app-payment-api'
  dependsOn: [
    daprPubSub
    seq
  ]
  params: {
    location: location
    seqFqdn: seq.outputs.fqdn
    containerAppsEnvironmentId: containerAppsEnvironment.outputs.id
  }
}

module seq 'modules/apps/seq.bicep' = {
  name: '${deployment().name}-app-seq'
  params: {
    location: location
    containerAppsEnvironmentId: containerAppsEnvironment.outputs.id
  }
}

module webshoppingAgg 'modules/apps/webshopping-agg.bicep' = {
  name: '${deployment().name}-app-webshopping-agg'
  dependsOn: [
    seq
  ]
  params: {
    location: location
    containerAppsEnvironmentId: containerAppsEnvironment.outputs.id
    containerAppsEnvironmentDomain: containerAppsEnvironment.outputs.domain
    seqFqdn: seq.outputs.fqdn
  }
}

module webshoppingGW 'modules/apps/webshopping-gw.bicep' = {
  name: '${deployment().name}-app-webshopping-gw'
  dependsOn: [
    seq
  ]
  params: {
    location: location
    containerAppsEnvironmentId: containerAppsEnvironment.outputs.id
    containerAppsEnvironmentDomain: containerAppsEnvironment.outputs.domain
  }
}

module webstatus 'modules/apps/webstatus.bicep' = {
  name: '${deployment().name}-app-webstatus'
  dependsOn: [
    seq
  ]
  params: {
    location: location
    containerAppsEnvironmentId: containerAppsEnvironment.outputs.id
    containerAppsEnvironmentDomain: containerAppsEnvironment.outputs.domain
  }
}
