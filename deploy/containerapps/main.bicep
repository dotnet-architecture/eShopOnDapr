param uniqueSeed string = '${resourceGroup().id}-${deployment().name}'
param location string = resourceGroup().location

module sqlServer 'modules/sql-server.bicep' = {
  name: '${deployment().name}-sql-server'
  params: {
    uniqueSeed: uniqueSeed
    location: location
  }
}

module containerAppsEnvironment 'modules/container-apps-env.bicep' = {
  name: '${deployment().name}-container-app-env'
  params: {
    location: location
    uniqueSeed: uniqueSeed
  }
}

module seq 'modules/seq.bicep' = {
  name: '${deployment().name}-seq'
  dependsOn: [
    containerAppsEnvironment
  ]
  params: {
    location: location
    containerAppsEnvironmentId: containerAppsEnvironment.outputs.containerAppsEnvironmentId
  }
}

module basketApi 'modules/basket-api.bicep' = {
  name: '${deployment().name}-basket-api'
  dependsOn: [
    containerAppsEnvironment
    seq
  ]
  params: {
    location: location
    containerAppsEnvironmentId: containerAppsEnvironment.outputs.containerAppsEnvironmentId
    containerAppsDomain: containerAppsEnvironment.outputs.domain
    seqFqdn: seq.outputs.fqdn
  }
}

module blazorClient 'modules/blazor-client.bicep' = {
  name: '${deployment().name}-blazor-client'
  dependsOn: [
    containerAppsEnvironment
    seq
  ]
  params: {
    location: location
    containerAppsEnvironmentId: containerAppsEnvironment.outputs.containerAppsEnvironmentId
    containerAppsDomain: containerAppsEnvironment.outputs.domain
    seqFqdn: seq.outputs.fqdn
  }
}

module catalogApi 'modules/catalog-api.bicep' = {
  name: '${deployment().name}-catalog-api'
  dependsOn: [
    containerAppsEnvironment
    seq
    sqlServer
  ]
  params: {
    location: location
    containerAppsEnvironmentId: containerAppsEnvironment.outputs.containerAppsEnvironmentId
    catalogDbConnectionString: sqlServer.outputs.catalogDbConnectionString
    seqFqdn: seq.outputs.fqdn
  }
}

module identityApi 'modules/identity-api.bicep' = {
  name: '${deployment().name}-identity-api'
  dependsOn: [
    containerAppsEnvironment
    seq
    sqlServer
  ]
  params: {
    location: location
    containerAppsEnvironmentId: containerAppsEnvironment.outputs.containerAppsEnvironmentId
    containerAppsDomain: containerAppsEnvironment.outputs.domain
    identityDbConnectionString: sqlServer.outputs.identityDbConnectionString
    seqFqdn: seq.outputs.fqdn
  }
}

module orderingApi 'modules/ordering-api.bicep' = {
  name: '${deployment().name}-ordering-api'
  dependsOn: [
    containerAppsEnvironment
    seq
    sqlServer
  ]
  params: {
    location: location
    containerAppsEnvironmentId: containerAppsEnvironment.outputs.containerAppsEnvironmentId
    containerAppsDomain: containerAppsEnvironment.outputs.domain
    orderingDbConnectionString: sqlServer.outputs.identityDbConnectionString
    seqFqdn: seq.outputs.fqdn
  }
}

module paymentApi 'modules/payment-api.bicep' = {
  name: '${deployment().name}-payment-api'
  dependsOn: [
    containerAppsEnvironment
    seq
  ]
  params: {
    location: location
    containerAppsEnvironmentId: containerAppsEnvironment.outputs.containerAppsEnvironmentId
    seqFqdn: seq.outputs.fqdn
  }
}

module webshoppingAgg 'modules/webshopping-agg.bicep' = {
  name: '${deployment().name}-webshopping-agg'
  dependsOn: [
    containerAppsEnvironment
    seq
  ]
  params: {
    location: location
    containerAppsEnvironmentId: containerAppsEnvironment.outputs.containerAppsEnvironmentId
    containerAppsDomain: containerAppsEnvironment.outputs.domain
    seqFqdn: seq.outputs.fqdn
  }
}

module webshoppingGW 'modules/webshopping-gw.bicep' = {
  name: '${deployment().name}-webshopping-gw'
  dependsOn: [
    containerAppsEnvironment
    seq
  ]
  params: {
    location: location
    containerAppsEnvironmentId: containerAppsEnvironment.outputs.containerAppsEnvironmentId
  }
}

module webstatus 'modules/webstatus.bicep' = {
  name: '${deployment().name}-webstatus'
  dependsOn: [
    containerAppsEnvironment
    seq
  ]
  params: {
    location: location
    containerAppsEnvironmentId: containerAppsEnvironment.outputs.containerAppsEnvironmentId
    containerAppsDomain: containerAppsEnvironment.outputs.domain
  }
}
