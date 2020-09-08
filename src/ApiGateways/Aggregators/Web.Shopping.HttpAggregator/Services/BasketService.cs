using System.Threading.Tasks;
using Dapr.Client;
using Dapr.Client.Http;
using Microsoft.eShopOnContainers.Web.Shopping.HttpAggregator.Models;

namespace Microsoft.eShopOnContainers.Web.Shopping.HttpAggregator.Services
{
    public class BasketService : IBasketService
    {
        private const string DaprAppId = "basket-api";

        private readonly DaprClient _daprClient;

        public BasketService(DaprClient daprClient)
        {
            _daprClient = daprClient;
        }

        public async Task<BasketData> GetById(string id)
        {
            return await _daprClient.InvokeMethodAsync<BasketData>(
                DaprAppId,
                $"api/v1/basket/{id}",
                new HTTPExtension { Verb = HTTPVerb.Get });
        }

        public Task UpdateAsync(BasketData currentBasket)
        {
            return _daprClient.InvokeMethodAsync(
                DaprAppId,
                "api/v1/basket",
                currentBasket);
        }
    }
}
