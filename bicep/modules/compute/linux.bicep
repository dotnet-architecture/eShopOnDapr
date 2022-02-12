param location string

param ubuntuVersion string
param vmSize string

@secure()
param adminUsername string

@secure()
param adminPassword string

param subnetId string

resource pip 'Microsoft.Network/publicIPAddresses@2021-05-01' = {
  name: 'pip-jumpbox'
  location: location
  sku: {
    name: 'Basic'
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

resource vm 'Microsoft.Compute/virtualMachines@2020-06-01' = {
  name: 'jumpbox'
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  properties: {    
    hardwareProfile: {
      vmSize: vmSize
    }
    storageProfile: {
      osDisk: {
        createOption: 'FromImage'
        managedDisk: {
          storageAccountType: 'Standard_LRS'
        }
      }
      imageReference: {
        publisher: 'Canonical'
        offer: 'UbuntuServer'
        sku: ubuntuVersion
        version: 'latest'
      }
    }
    networkProfile: {
      networkInterfaces: [
        {
          id: nic.id
        }
      ]
    }
    osProfile: {
      computerName: 'jumpbox'
      adminUsername: adminUsername
      adminPassword: adminPassword
      linuxConfiguration: {        
        patchSettings: {
          patchMode: 'ImageDefault'
        }
      }
      customData: loadFileAsBase64('jumpbox-cloud-init.yaml')
    }
    diagnosticsProfile: {
      bootDiagnostics: {
        enabled: true
      }
    }
  }
}


output vmName string = vm.name
output privateIp string = nic.properties.ipConfigurations[0].properties.privateIPAddress
