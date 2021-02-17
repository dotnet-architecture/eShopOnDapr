using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.eShopOnContainers.Web.Shopping.HttpAggregator.Models;

namespace Microsoft.eShopOnContainers.Web.Shopping.HttpAggregator.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly HttpClient _httpClient;

        public CatalogService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public Task<CatalogItem> GetCatalogItemAsync(int id)
        {
            var requestUri = $"api/v1/catalog/items/{id}";
        
            return _httpClient.GetFromJsonAsync<CatalogItem>(requestUri);
        }

        public Task<IEnumerable<CatalogItem>> GetCatalogItemsAsync(IEnumerable<int> ids)
        {
            var requestUri = $"api/v1/catalog/items?ids={string.Join(",", ids)}";
        
            return _httpClient.GetFromJsonAsync<IEnumerable<CatalogItem>>(requestUri);
        }
    }
}
