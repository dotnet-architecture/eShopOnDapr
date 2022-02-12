param location string
param subnetId string
param aadAdminGroupId string
param acrName string
param acrId string
param aksAzureCniSettings object

param guidValue string = newGuid()

var kubernetesVersion = '1.22.6'
var aksName = 'aksdevops'


resource aks 'Microsoft.ContainerService/managedClusters@2021-10-01' = {
  name: aksName
  location: location
  identity: {
    type: 'SystemAssigned'    
  }
  properties: {
    kubernetesVersion: kubernetesVersion
    enableRBAC: true
    dnsPrefix: '${aksName}-dns'
    agentPoolProfiles: [
      {
        name: 'systempool'
        osDiskSizeGB: 0
        count: 2
        enableAutoScaling: false
        vmSize: 'Standard_DS2_v2'
        osType: 'Linux'        
        type: 'VirtualMachineScaleSets'
        mode: 'System'
        maxPods: 110
        availabilityZones: null
        vnetSubnetID: subnetId
      }
      {
        name: 'workloadpool'
        osDiskSizeGB: 0
        count: 3
        enableAutoScaling: true
        minCount: 1
        maxCount: 5
        vmSize: 'Standard_DS2_v2'
        osType: 'Linux'        
        type: 'VirtualMachineScaleSets'
        mode: 'User'
        maxPods: 110
        availabilityZones: null
        nodeTaints: [
          'type=workload:NoSchedule'
        ]
        vnetSubnetID: subnetId
      }      
    ]
    networkProfile: {
      loadBalancerSku: 'standard'
      networkPlugin: 'azure'
      networkPolicy: 'calico'
      serviceCidr: aksAzureCniSettings.serviceCidr
      dnsServiceIP: aksAzureCniSettings.dnsServiceIP
      dockerBridgeCidr: aksAzureCniSettings.dockerBridgeCidr
    }
    aadProfile: {
      adminGroupObjectIDs: [
        aadAdminGroupId
      ]
    }
    apiServerAccessProfile: {
      enablePrivateCluster: true
    }
    addonProfiles: {
      azureKeyvaultSecretsProvider: {
        enabled: true
      }
    }
  }
}

resource rbacAcr 'Microsoft.ContainerRegistry/registries/providers/roleAssignments@2018-09-01-preview' = {
  name: '${acrName}/Microsoft.Authorization/${guidValue}'
  properties: {
    principalId: aks.identity.principalId
    principalType: 'ServicePrincipal'
    roleDefinitionId: '/subscriptions/${subscription().subscriptionId}/providers/Microsoft.Authorization/roleDefinitions/b24988ac-6180-42a0-ab88-20f7382dd24c'
    scope: acrId
  }
}

