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
    subnets: [
      {
        name: '${vnetSettings.subnets[0].name}'
        properties: {
          addressPrefix: vnetSettings.subnets[0].addressPrefix
          privateEndpointNetworkPolicies: 'Enabled'
          privateLinkServiceNetworkPolicies: 'Enabled'
        }          
      }
      {
        name: '${vnetSettings.subnets[1].name}'
        properties: {
          addressPrefix: vnetSettings.subnets[1].addressPrefix
          networkSecurityGroup: {
            id: nsgRunner.id
          }
        }        
      }
      {
        name: '${vnetSettings.subnets[2].name}'
        properties: {
          addressPrefix: vnetSettings.subnets[2].addressPrefix
          privateEndpointNetworkPolicies: 'Disabled'
          privateLinkServiceNetworkPolicies: 'Enabled'
        }        
      }
    ]
  }
}

output vnetId string = virtualNetwork.id
output virtualNetworkName string = virtualNetwork.name
output jumpboxSubnetId string = virtualNetwork.properties.subnets[1].id
output prvEndpointSubnetId string = virtualNetwork.properties.subnets[2].id
output aksSubnetId string = virtualNetwork.properties.subnets[0].id
