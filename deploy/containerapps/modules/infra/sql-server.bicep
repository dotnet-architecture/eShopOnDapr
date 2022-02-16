param location string
param uniqueSeed string
param sqlServerName string = 'sql-${uniqueString(uniqueSeed)}'
param sqlAdministratorLogin string = 'server_admin'
param sqlAdministratorLoginPassword string = 'Pass@word'
param catalogDbName string = 'Microsoft.eShopOnDapr.Services.CatalogDb'
param identityDbName string = 'Microsoft.eShopOnDapr.Services.IdentityDb'
param orderingDbName string = 'Microsoft.eShopOnDapr.Services.OrderingDb'

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
    name: catalogDbName
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

output catalogDbConnectionString string = 'Server=tcp:${sqlServerName}${environment().suffixes.sqlServerHostname},1433;Initial Catalog=${catalogDbName};Persist Security Info=False;User ID=${sqlAdministratorLogin};Password=${sqlAdministratorLoginPassword};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'
output identityDbConnectionString string = 'Server=tcp:${sqlServerName}${environment().suffixes.sqlServerHostname},1433;Initial Catalog=${identityDbName};Persist Security Info=False;User ID=${sqlAdministratorLogin};Password=${sqlAdministratorLoginPassword};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'
output orderingDbConnectionString string = 'Server=tcp:${sqlServerName}${environment().suffixes.sqlServerHostname},1433;Initial Catalog=${orderingDbName};Persist Security Info=False;User ID=${sqlAdministratorLogin};Password=${sqlAdministratorLoginPassword};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'
