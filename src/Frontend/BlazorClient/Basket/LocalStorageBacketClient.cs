using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace eShopOnDapr.BlazorClient.Basket
{
    public class LocalStorageBasketClient
    {
        private const string BASKET_KEY = "eshop.basket";

        private readonly IJSRuntime _js;

        public LocalStorageBasketClient(IJSRuntime js)
        {
            _js = js ?? throw new ArgumentNullException(nameof(js));
        }

        public async Task<IEnumerable<BasketItem>> LoadItemsAsync()
        {
            Console.WriteLine("---> LocalStorageBasketStateStore.LoadBasketAsync");

            var state = await _js.InvokeAsync<string>(
                "sessionStorage.getItem", BASKET_KEY);

            if (state != null)
            {
                return JsonSerializer.Deserialize<IEnumerable<BasketItem>>(
                    state);
            }

            return Enumerable.Empty<BasketItem>();
        }

        public async Task SaveBasketAsync(IEnumerable<BasketItem> items)
        {
            Console.WriteLine("---> LocalStorageBasketStateStore.SaveBasketAsync");

            var state = JsonSerializer.Serialize(items);

            await _js.InvokeVoidAsync(
                "sessionStorage.setItem", BASKET_KEY, state);
        }
    }
}
