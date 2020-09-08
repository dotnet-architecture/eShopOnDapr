using Microsoft.eShopOnContainers.Web.Shopping.HttpAggregator.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapr.Client;
using Dapr.Client.Http;

namespace Microsoft.eShopOnContainers.Web.Shopping.HttpAggregator.Services
{
    public class CatalogService : ICatalogService
    {
        private const string DaprAppId = "catalog-api";

        private readonly DaprClient _daprClient;

        public CatalogService(DaprClient daprClient)
        {
            _daprClient = daprClient;
        }

        public async Task<CatalogItem> GetCatalogItemAsync(int id)
        {
            return await _daprClient.InvokeMethodAsync<CatalogItem>(
                DaprAppId,
                $"api/v1/catalog/items/{id}",
                new HTTPExtension { Verb = HTTPVerb.Get });
        }

        public async Task<IEnumerable<CatalogItem>> GetCatalogItemsAsync(IEnumerable<int> ids)
        {
            return await _daprClient.InvokeMethodAsync<IEnumerable<CatalogItem>>(
                DaprAppId,
                "api/v1/catalog/items",
                new HTTPExtension
                { 
                    QueryString = new Dictionary<string, string>
                    {
                        ["pageSize"] = "10",
                        ["pageIndex"] = "1",
                        ["ids"] = string.Join(",", ids),
                    },
                    Verb = HTTPVerb.Get 
                });
        }
    }
}
