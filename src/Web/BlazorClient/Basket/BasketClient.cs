using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.eShopOnDapr.BlazorClient.Ordering;

namespace Microsoft.eShopOnDapr.BlazorClient.Basket
{
    public class BasketClient
    {
        private readonly HttpClient _httpClient;

        public BasketClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<BasketItem>> GetItemsAsync()
        {
            var basket = await _httpClient.GetFromJsonAsync<BasketData>(
                "b/api/v1/basket/");

            return basket.Items;
        }

        public async Task<IEnumerable<BasketItem>> SaveItemsAsync(IEnumerable<BasketItem> items)
        {
            var request = new BasketData(items);

            // Save items is a request to the Aggregator service.
            var response = await _httpClient.PostAsJsonAsync(
                "api/v1/basket/",
                request);

            response.EnsureSuccessStatusCode();

            var basketData = await response.Content.ReadFromJsonAsync<BasketData>();
            return basketData.Items;
        }

        public async Task CheckoutAsync(BasketCheckout basketCheckout)
        {
            var response = await _httpClient.PostAsJsonAsync(
                "b/api/v1/basket/checkout",
                basketCheckout);

            response.EnsureSuccessStatusCode();
        }
    }
}
