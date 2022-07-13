param location string
param uniqueSeed string
param containerAppsEnvironmentName string = 'containerappenv-${uniqueString(uniqueSeed)}'
param logAnalyticsWorkspaceName string = 'loganalytics-${uniqueString(uniqueSeed)}'

resource logAnalyticsWorkspace 'Microsoft.OperationalInsights/workspaces@2020-03-01-preview' = {
  name: logAnalyticsWorkspaceName
  location: location
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

resource containerAppsEnvironment 'Microsoft.App/managedEnvironments@2022-03-01' = {
  name: containerAppsEnvironmentName
  location: location
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

output id string = containerAppsEnvironment.id
output name string = containerAppsEnvironmentName
output domain string = containerAppsEnvironment.properties.defaultDomain
