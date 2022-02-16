param location string
param suffix string

resource serviceBusNamespace 'Microsoft.ServiceBus/namespaces@2017-04-01' = {
  name: 'srvbus-${suffix}'
  location: location
  sku: {
    name: 'Standard'
  }
  properties: {}
}
