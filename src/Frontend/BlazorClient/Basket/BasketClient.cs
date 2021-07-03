using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using eShopOnDapr.BlazorClient.Ordering;

namespace eShopOnDapr.BlazorClient.Basket
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
            var basket = await _httpClient.GetFromJsonAsync<CustomerBasket2>(
                string.Empty);

            return basket.Items;
        }

        public async Task SaveItemsAsync(IEnumerable<BasketItem> items)
        {
            var request = new CustomerBasket2
            {
//                BuyerId = buyerId,
                Items = items.ToList()
            };

            var response = await _httpClient.PostAsJsonAsync(string.Empty, request);

            response.EnsureSuccessStatusCode();

//            return await response.Content.ReadFromJsonAsync<Basket>();
        }

        public async Task CheckoutAsync(BasketCheckout basketCheckout)
        {
            var response = await _httpClient.PostAsJsonAsync("checkout", basketCheckout);

            response.EnsureSuccessStatusCode();
        }
    }
}
