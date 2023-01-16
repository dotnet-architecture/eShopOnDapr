param uniqueSeed string
param location string
param identityName string = 'id-${uniqueString(uniqueSeed)}'

resource managedIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2022-01-31-preview' = {
  name: identityName
  location: location
}

output identityId string = managedIdentity.id
output identityClientId string = managedIdentity.properties.clientId
output identityObjectId string = managedIdentity.properties.principalId

