param location string
param containerAppsEnvironmentId string
param containerAppsDomain string

resource containerApp 'Microsoft.Web/containerApps@2021-03-01' = {
  name: 'webstatus'
  location: location
  properties: {
    kubeEnvironmentId: containerAppsEnvironmentId
    template: {
      containers: [
        {
          name: 'webstatus'
          image: 'xabarilcoding/healthchecksui:5.0.0'
          env: [
            {
              name: 'ASPNETCORE_URLS'
              value: 'http://0.0.0.0:80'
            }
            {
              name: 'HealthChecksUI__HealthChecks__0__Name'
              value: 'Basket API'
            }  
            {
              name: 'HealthChecksUI__HealthChecks__0__Uri'
              value: 'https://basket-api.${containerAppsDomain}/hc'
            }
            {
              name: 'HealthChecksUI__HealthChecks__1__Name'
              value: 'Catalog API'
            }  
            {
              name: 'HealthChecksUI__HealthChecks__1__Uri'
              value: 'https://catalog-api.${containerAppsDomain}/hc'
            }
            {
              name: 'HealthChecksUI__HealthChecks__2__Name'
              value: 'Identity API'
            }  
            {
              name: 'HealthChecksUI__HealthChecks__2__Uri'
              value: 'https://identity-api.${containerAppsDomain}/hc'
            }
            {
              name: 'HealthChecksUI__HealthChecks__3__Name'
              value: 'Ordering API'
            }  
            {
              name: 'HealthChecksUI__HealthChecks__3__Uri'
              value: 'https://ordering-api.${containerAppsDomain}/hc'
            }
            {
              name: 'HealthChecksUI__HealthChecks__4__Name'
              value: 'Payment API'
            }  
            {
              name: 'HealthChecksUI__HealthChecks__4__Uri'
              value: 'https://payment-api.${containerAppsDomain}/hc'
            }
            {
              name: 'HealthChecksUI__HealthChecks__5__Name'
              value: 'Web Shopping Aggregator'
            }  
            {
              name: 'HealthChecksUI__HealthChecks__5__Uri'
              value: 'https://webshopping-agg.${containerAppsDomain}/hc'
            }
            {
              name: 'HealthChecksUI__HealthChecks__6__Name'
              value: 'Blazor UI Host'
            }  
            {
              name: 'HealthChecksUI__HealthChecks__6__Uri'
              value: 'https://blazor-client.${containerAppsDomain}/hc'
            }
          ]
        }
      ]
      scale: {
        minReplicas: 1
      }
    }
    configuration: {
      activeResivionsMode: 'single'
      ingress: {
        external: true
        targetPort: 80
      }
    }
  }
}

//output fqdn string = containerApp.properties.configuration.ingress.fqdn
