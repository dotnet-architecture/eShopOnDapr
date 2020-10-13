using System.Threading.Tasks;
using Dapr.Client;
using Microsoft.eShopOnContainers.Web.Shopping.HttpAggregator.Models;

namespace Microsoft.eShopOnContainers.Web.Shopping.HttpAggregator.Services
{
        public class OrderingService : IOrderingService
    {
        private const string DaprAppId = "ordering-api";

        private readonly DaprClient _daprClient;

        public OrderingService(DaprClient daprClient)
        {
            _daprClient = daprClient;
        }

        public async Task<OrderData> GetOrderDraftAsync(BasketData basketData)
        {
            return await _daprClient.InvokeMethodAsync<BasketData, OrderData>(
                DaprAppId,
                $"api/v1/orders/draft",
                basketData);
        }
    }
}
