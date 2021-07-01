using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace eShopOnDapr.BlazorClient.Basket
{
    public class ApiBasketClient
    {
        private readonly HttpClient _httpClient;

        public ApiBasketClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<BasketItem>> LoadItemsAsync(string buyerId)
        {
            Console.WriteLine("BASKET::LOAD (AUTH)");

            var basket = await _httpClient.GetFromJsonAsync<CustomerBasket2>(
                buyerId);

            return basket.Items;
        }

        public async Task SaveItemsAsync(
            string buyerId, IEnumerable<BasketItem> items)
        {
            var request = new CustomerBasket2
            {
                BuyerId = buyerId,
                Items = items.ToList()
            };

            var response = await _httpClient.PostAsJsonAsync(string.Empty, request);

            response.EnsureSuccessStatusCode();

//            return await response.Content.ReadFromJsonAsync<Basket>();
        }
    }
}
