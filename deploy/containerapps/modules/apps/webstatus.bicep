param location string

param containerAppsEnvironmentId string
param containerAppsEnvironmentDomain string

resource containerApp 'Microsoft.App/containerApps@2022-03-01' = {
  name: 'webstatus'
  location: location
  properties: {
    managedEnvironmentId: containerAppsEnvironmentId
    template: {
      containers: [
        {
          name: 'webstatus'
          image: 'eshopdapr/webstatus:latest'
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
              value: 'http://basket-api.internal.${containerAppsEnvironmentDomain}/hc'
            }
            {
              name: 'HealthChecksUI__HealthChecks__1__Name'
              value: 'Catalog API'
            }  
            {
              name: 'HealthChecksUI__HealthChecks__1__Uri'
              value: 'http://catalog-api.internal.${containerAppsEnvironmentDomain}/hc'
            }
            {
              name: 'HealthChecksUI__HealthChecks__2__Name'
              value: 'Identity API'
            }  
            {
              name: 'HealthChecksUI__HealthChecks__2__Uri'
              value: 'https://identity-api.${containerAppsEnvironmentDomain}/hc'
            }
            {
              name: 'HealthChecksUI__HealthChecks__3__Name'
              value: 'Ordering API'
            }  
            {
              name: 'HealthChecksUI__HealthChecks__3__Uri'
              value: 'http://ordering-api.internal.${containerAppsEnvironmentDomain}/hc'
            }
            {
              name: 'HealthChecksUI__HealthChecks__4__Name'
              value: 'Payment API'
            }  
            {
              name: 'HealthChecksUI__HealthChecks__4__Uri'
              value: 'http://payment-api.internal.${containerAppsEnvironmentDomain}/hc'
            }
            {
              name: 'HealthChecksUI__HealthChecks__5__Name'
              value: 'Web Shopping Aggregator'
            }  
            {
              name: 'HealthChecksUI__HealthChecks__5__Uri'
              value: 'http://webshopping-agg.internal.${containerAppsEnvironmentDomain}/hc'
            }
            {
              name: 'HealthChecksUI__HealthChecks__6__Name'
              value: 'Blazor UI Host'
            }  
            {
              name: 'HealthChecksUI__HealthChecks__6__Uri'
              value: 'https://blazor-client.${containerAppsEnvironmentDomain}/hc'
            }
          ]
        }
      ]
      scale: {
        minReplicas: 1
        maxReplicas: 1
      }
    }
    configuration: {
      activeRevisionsMode: 'single'
      ingress: {
        external: true
        targetPort: 80
      }
    }
  }
}
