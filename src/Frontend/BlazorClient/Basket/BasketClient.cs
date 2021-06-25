using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace eShopOnDapr.BlazorClient.Basket
{
    public class BasketClient
    {
        private readonly HttpClient httpClient;

        public BasketClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task InitializeAsync()
        {
            Console.WriteLine("BasketClient.InitializeAsync");

            await httpClient.GetFromJsonAsync<IEnumerable<BasketItem>>(string.Empty);
        }

        public async Task<Basket> SetBasket<Basket>(Basket basket)
        {
            var response = await httpClient.PostAsJsonAsync(string.Empty, basket);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<Basket>();
        }
    }
}
