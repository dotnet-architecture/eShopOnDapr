param location string
param suffix string

resource strVirtualMachine 'Microsoft.Storage/storageAccounts@2021-06-01' = {
  name: 'strv${suffix}'
  location: location
  tags: {
    'description': 'vm storage diagnostics'
  }
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'  
}

output strVmName string = strVirtualMachine.name

