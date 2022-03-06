param location string
param suffix string

// resource acr 'Microsoft.ContainerRegistry/registries@2021-09-01' = {
//   name: 'acr${suffix}'
//   location: location
//   sku: {
//     name: 'Premium'
//   }
//   properties: {
//     adminUserEnabled: true
//     networkRuleSet: {
//       defaultAction: 'Allow'
//       ipRules: [
        
//       ]
//     }
//     publicNetworkAccess: 'Disabled'
//     networkRuleBypassOptions: 'AzureServices'
//   }
// }

resource acr 'Microsoft.ContainerRegistry/registries@2021-09-01' = {
  name: 'acr${suffix}'
  location: location
  sku: {
    name: 'Premium'
  }
  properties: {
    adminUserEnabled: true
    publicNetworkAccess: 'Enabled'
    networkRuleBypassOptions: 'AzureServices'
  }
}



output acrId string = acr.id
output acrName string = '${acr.name}.azurecr.io'
