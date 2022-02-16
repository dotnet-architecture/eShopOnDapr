param location string
param suffix string

resource redisCache 'Microsoft.Cache/redis@2021-06-01' = {
  name: 'redis-${suffix}'
  location: location
  properties: {
    sku: {
      capacity: 0
      family: 'C'
      name: 'Basic'
    }
  }
}
