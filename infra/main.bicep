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
param basketapiImageName string = ''

@description('The image name for the catalog api service')
param catalogapiImageName string = ''

@description('The image name for the blazor client service')
param blazorclientImageName string = ''

@description('The image name for the identity api service')
param identityapiImageName string = ''

@description('The image name for the ordering api service')
param orderingapiImageName string = ''

@description('The image name for the payment api service')
param paymentapiImageName string = ''

@description('The image name for the webshopping aggregator service')
param webshoppingaggImageName string = ''

@description('The image name for the webshopping gateway service')
param webshoppinggwImageName string = ''

@description('The image name for the web status health check service')
param webstatusImageName string = ''

@description('If UseDockerHubImages value is true then deployment will use docker hub images(recommended for use without azd)')
param useDockerHubImages string = 'false'

var isUsingDockerHubImages = bool(useDockerHubImages) == true

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
    basketapiImageName: isUsingDockerHubImages ? 'eshopdapr/basket.api:20220331': basketapiImageName
    catalogapiImageName: isUsingDockerHubImages ? 'eshopdapr/catalog.api:20220331' : catalogapiImageName 
    orderingapiImageName: isUsingDockerHubImages ? 'eshopdapr/ordering.api:20220331': orderingapiImageName
    paymentapiImageName: isUsingDockerHubImages ? 'eshopdapr/payment.api:20220331' : paymentapiImageName
    identityapiImageName: isUsingDockerHubImages ? 'eshopdapr/identity.api:20220331' : identityapiImageName
    blazorclientImageName: isUsingDockerHubImages ? 'eshopdapr/blazor.client:20220331' : blazorclientImageName
    webshoppingaggImageName: isUsingDockerHubImages ? 'eshopdapr/webshoppingagg:20220331' : webshoppingaggImageName
    webshoppinggwImageName: isUsingDockerHubImages ? 'eshopdapr/webshoppingapigw:20220331' : webshoppinggwImageName
    webstatusImageName: isUsingDockerHubImages ? 'eshopdapr/webstatus:latest' : webstatusImageName
    tags: tags
  }
}

output AZURE_COSMOS_CONNECTION_STRING_KEY string = resources.outputs.AZURE_COSMOS_CONNECTION_STRING_KEY
output AZURE_COSMOS_DATABASE_NAME string = resources.outputs.AZURE_COSMOS_DATABASE_NAME
output AZURE_KEY_VAULT_ENDPOINT string = resources.outputs.AZURE_KEY_VAULT_ENDPOINT
output APPINSIGHTS_INSTRUMENTATIONKEY string = resources.outputs.APPINSIGHTS_INSTRUMENTATIONKEY
output AZURE_CONTAINER_REGISTRY_ENDPOINT string = resources.outputs.AZURE_CONTAINER_REGISTRY_ENDPOINT
output AZURE_CONTAINER_REGISTRY_NAME string = resources.outputs.AZURE_CONTAINER_REGISTRY_NAME
output REACT_APP_APPINSIGHTS_INSTRUMENTATIONKEY string = resources.outputs.APPINSIGHTS_INSTRUMENTATIONKEY
output AZURE_CATALOG_DB_CONN_STRING string = resources.outputs.CATALOG_DB_CONN_STRING
output AZURE_IDENTITY_DB_CONN_STRING string = resources.outputs.IDENTITY_DB_CONN_STRING
output AZURE_ORDERING_DB_CONN_STRING string = resources.outputs.ORDERING_DB_CONN_STRING
output APP_SEQ_FQDN string = resources.outputs.SEQ_FQDN
output WEB_BLAZORCLIENT string = resources.outputs.WEB_BLAZORCLIENT
output WEBSTATUS_BASE_URL string = resources.outputs.WEBSTATUS_URI
