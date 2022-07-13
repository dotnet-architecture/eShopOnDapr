targetScope = 'subscription'

@minLength(1)
@maxLength(64)
@description('Name of the the environment which is used to generate a short unqiue hash used in all resources.')
param name string

@minLength(1)
@description('Primary location for all resources')
param location string

@description('Id of the user or app to assign application roles')
param principalId string = ''

@description('The image name for the basket api service')
param basketApiImageName string = ''

@description('The image name for the catalog api service')
param catalogApiImageName string = ''

@description('The image name for the blazor client service')
param blazorClientImageName string = ''

@description('The image name for the identity api service')
param identityApiImageName string = ''

@description('The image name for the ordering api service')
param orderingApiImageName string = ''

@description('The image name for the payment api service')
param paymentApiImageName string = ''

@description('The image name for the webshopping aggregator service')
param webshoppingAggImageName string = ''

@description('The image name for the webshopping gateway service')
param webshoppingGwImageName string = ''

@description('The image name for the web status health check service')
param webstatusImageName string = ''

resource resourceGroup 'Microsoft.Resources/resourceGroups@2020-06-01' = {
  name: '${name}-rg'
  location: location
}

var resourceToken = toLower(uniqueString(subscription().id, name, location))
var tags = {
  'azd-env-name': name
}

module resources './resources.bicep' = {
  name: 'resources-${resourceToken}'
  scope: resourceGroup
  params: {
    name: name
    location: location
    principalId: principalId
    resourceToken: resourceToken
    basketapiImageName: basketApiImageName
    blazorclientImageName: blazorClientImageName
    catalogapiImageName: catalogApiImageName
    identityapiImageName: identityApiImageName
    orderingapiImageName: orderingApiImageName
    paymentapiImageName: paymentApiImageName
    webshoppingaggImageName: webshoppingAggImageName
    webshoppinggwImageName: webshoppingGwImageName
    webstatusImageName: webstatusImageName
    tags: tags
  }
}

output AZURE_COSMOS_CONNECTION_STRING_KEY string = resources.outputs.AZURE_COSMOS_CONNECTION_STRING_KEY
output AZURE_COSMOS_DATABASE_NAME string = resources.outputs.AZURE_COSMOS_DATABASE_NAME
output AZURE_KEY_VAULT_ENDPOINT string = resources.outputs.AZURE_KEY_VAULT_ENDPOINT
output APPINSIGHTS_INSTRUMENTATIONKEY string = resources.outputs.APPINSIGHTS_INSTRUMENTATIONKEY
output AZURE_CONTAINER_REGISTRY_ENDPOINT string = resources.outputs.AZURE_CONTAINER_REGISTRY_ENDPOINT
output AZURE_CONTAINER_REGISTRY_NAME string = resources.outputs.AZURE_CONTAINER_REGISTRY_NAME
output REACT_APP_WEB_BASE_URL string = resources.outputs.WEB_URI
output REACT_APP_API_BASE_URL string = resources.outputs.API_URI
output REACT_APP_APPINSIGHTS_INSTRUMENTATIONKEY string = resources.outputs.APPINSIGHTS_INSTRUMENTATIONKEY
