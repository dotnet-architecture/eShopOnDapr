param location string
param containerAppsEnvironmentId string

resource containerApp 'Microsoft.Web/containerApps@2021-03-01' = {
  name: 'echo'
  location: location
  properties: {
    kubeEnvironmentId: containerAppsEnvironmentId
    template: {
      containers: [
        {
          name: 'echo'
          image: 'mendhak/http-https-echo:23'
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
        targetPort: 8080
        allowInsecure: true
      }
    }
  }
}
