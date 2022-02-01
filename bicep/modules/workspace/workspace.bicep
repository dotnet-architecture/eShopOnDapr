param location string
param suffix string

resource workspace 'Microsoft.OperationalInsights/workspaces@2021-06-01' = {
  name: 'log-${suffix}'
  location: location
  properties: {
    sku: {
      name: 'PerGB2018'
    }
  }
}

output workSpaceId string = workspace.id
