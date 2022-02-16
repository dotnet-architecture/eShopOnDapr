param location string

@secure()
param administratorLogin string

@secure()
param administratorLoginPassword string

var serverName = uniqueString('sql', resourceGroup().id)
var sqlCatalogDBName = 'catalogDB'
var sqlOrderingDBName = 'orderingDB'

resource server 'Microsoft.Sql/servers@2019-06-01-preview' = {
  name: serverName
  location: location
  properties: {
    administratorLogin: administratorLogin
    administratorLoginPassword: administratorLoginPassword
  }
}

resource sqlDBCatalog 'Microsoft.Sql/servers/databases@2020-08-01-preview' = {
  name: '${server.name}/${sqlCatalogDBName}'
  location: location
  sku: {
    name: 'Standard'
    tier: 'Standard'
  }
}

resource sqlDBOrdering 'Microsoft.Sql/servers/databases@2020-08-01-preview' = {
  name: '${server.name}/${sqlOrderingDBName}'
  location: location
  sku: {
    name: 'Standard'
    tier: 'Standard'
  }
}
