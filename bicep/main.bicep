@description('The location where the Azure resources will be deployed')
param location string

@description('The settings of the virtual network')
param vnetSettings object = {
  addressPrefixe: '10.0.0.0/8'  
  subnets: [
    {
      name: 'snet-aks'
      addressPrefix: '10.0.0.0/16'
    }
    {
      name: 'snet-jumpbox'
      addressPrefix: '10.0.0.0/27'
    }
  ]
}

@description('The username of the admin of the VM')
@secure()
param adminUsername string

@description('The admin password to connect to the VM')
@secure()
param adminPassword string

@description('The version of Ubuntu OS')
param ubuntuVersion string

@description('The size of the VM')
param vmSize string

var suffix = uniqueString(resourceGroup().id)

module vnet 'modules/network/vnet.bicep' = {
  name: 'vnet'
  params: {
    location: location 
    suffix: suffix
    vnetSettings: vnetSettings    
  }
}

module acr 'modules/acr/registry.bicep' = {
  name: 'acr'
  params: {
    location: location
    suffix: suffix
  }
}

module privateZoneAcr 'modules/dns/privateACRDnzZone.bicep' = {
  name: 'privateZoneAcr'
  params: {
    location: location
    vnetName: vnet.outputs.virtualNetworkName
    privateLinkResourceId: acr.outputs.acrId
    subnetId: vnet.outputs.prvEndpointSubnetId
    vnetId: vnet.outputs.vnetId
  }
}

module storage 'modules/storage/storage.bicep' = {
  name: 'storage'
  params: {
    location: location
    suffix: suffix
  }
}

module jumpbox 'modules/compute/linux.bicep' = {
  name: 'jumpbox'
  params: {
    location: location    
    subnetId: vnet.outputs.jumpboxSubnetId
    adminPassword: adminPassword
    adminUsername: adminUsername
    ubuntuVersion: ubuntuVersion
    vmSize: vmSize
  }
}

output jumpboxVmName string = jumpbox.outputs.vmName
