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

@description('The SKU Family of the VM')
param vmSku string

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
    privateLinkResourceId: acr.outputs.acrId
    subnetId: vnet.outputs.prvEndpointSubnetId
    vnetId: vnet.outputs.vnetId
  }
}

module jumpbox 'modules/compute/linux.bicep' = {
  name: 'jumpbox'
  params: {
    location: location
    subnetId: vnet.outputs.jumpboxSubnetId
    vmSku: vmSku
  }
}