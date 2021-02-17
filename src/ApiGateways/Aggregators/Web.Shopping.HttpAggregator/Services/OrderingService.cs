using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.eShopOnContainers.Web.Shopping.HttpAggregator.Models;

namespace Microsoft.eShopOnContainers.Web.Shopping.HttpAggregator.Services
{
        public class OrderingService : IOrderingService
    {
        private readonly HttpClient _httpClient;

        public OrderingService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<OrderData> GetOrderDraftAsync(BasketData basketData)
        {
            var requestUri = "api/v1/orders/draft";
        
            var response = await _httpClient.PostAsJsonAsync(requestUri, basketData);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<OrderData>();
        }
    }
}
