using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace eShopOnDapr.BlazorClient.Ordering
{
    public class OrderClient
    {
        private readonly HttpClient _httpClient;

        public OrderClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public Task<IEnumerable<OrderSummary>> GetOrdersAsync()
            => _httpClient.GetFromJsonAsync<IEnumerable<OrderSummary>>(string.Empty);

        public Task<Order> GetOrderDetailsAsync(int orderNumber)
            => _httpClient.GetFromJsonAsync<Order>(orderNumber.ToString());
    }
}
