param location string
// param suffix string

// param ubuntuVersion string
// param vmSize string
param vmSku string

param subnetId string

resource pip 'Microsoft.Network/publicIPAddresses@2021-05-01' = {
  name: 'pip-jumpbox'
  location: location
  sku: {
    name: vmSku
  }
  properties: {
    publicIPAllocationMethod: 'Dynamic'
    publicIPAddressVersion: 'IPv4'
  }
}

resource nic 'Microsoft.Network/networkInterfaces@2021-05-01' = {
  name: 'nic-jumpbox'
  location: location
  properties: {
    ipConfigurations: [
      {
        name: 'ipconfig-jumpbox'
        properties: {
          subnet: {
            id: subnetId
          }
          privateIPAllocationMethod: 'Dynamic'
          publicIPAddress: {
            id: pip.id
          }
        }
      }
    ]
  }
}


