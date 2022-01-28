param location string
param suffix string

resource acr 'Microsoft.ContainerRegistry/registries@2021-09-01' = {
  name: 'acr-${suffix}'
  location: location
  sku: {
    name: 'Premium'
  }
  properties: {
    adminUserEnabled: false
  }
}
