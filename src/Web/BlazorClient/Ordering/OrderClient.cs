using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Microsoft.eShopOnDapr.BlazorClient.Ordering
{
    public class OrderClient
    {
        private readonly HttpClient _httpClient;

        public OrderClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public Task<IEnumerable<OrderSummary>> GetOrdersAsync()
            => _httpClient.GetFromJsonAsync<IEnumerable<OrderSummary>>(
                "o/api/v1/orders/");

        public Task<Order> GetOrderDetailsAsync(int orderNumber)
            => _httpClient.GetFromJsonAsync<Order>(
                $"o/api/v1/orders/{orderNumber}");

        public async Task CancelOrderAsync(int orderNumber)
        {
            var response = await _httpClient.PutAsync(
                $"o/api/v1/orders/{orderNumber}/cancel",
                null);

            response.EnsureSuccessStatusCode();
        }
    }
}
