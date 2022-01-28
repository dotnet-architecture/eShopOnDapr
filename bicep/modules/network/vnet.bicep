param location string
param suffix string
param vnetSettings object

var vnetName = 'vnet-aks-${suffix}'

resource nsgRunner 'Microsoft.Network/networkSecurityGroups@2021-02-01' = {
  name: 'nsg-runner'
  location: location
}

resource virtualNetwork 'Microsoft.Network/virtualNetworks@2021-05-01' = {
  name: vnetName
  location: location
  properties: {
    addressSpace: {
      addressPrefixes: [
        vnetSettings.addressPrefixe
      ]
    }
  }
}



resource subnetAks 'Microsoft.Network/virtualNetworks/subnets@2020-06-01' = {
  name: '${virtualNetwork.name}/${vnetSettings.subnets[0].name}'
  properties: {
    addressPrefix: vnetSettings.subnets[0].addressPrefix
    privateEndpointNetworkPolicies: 'Enabled'
    privateLinkServiceNetworkPolicies: 'Enabled'    
  }
}

resource subnetRunner 'Microsoft.Network/virtualNetworks/subnets@2020-06-01' = {
  name: '${virtualNetwork.name}/${vnetSettings.subnets[1].name}'
  properties: {
    addressPrefix: vnetSettings.subnets[1].addressPrefix
    networkSecurityGroup: {
      id: nsgRunner.id
    }
  }
}
