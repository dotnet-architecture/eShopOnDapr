using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace eShopOnDapr.BlazorClient.Basket
{
    public class StatefulBasket
    {
        private IBasketStateStore _stateStore;
        private List<BasketItem> _items;

        public StatefulBasket(IBasketStateStore stateStore)
        {
            _stateStore = stateStore ?? throw new ArgumentNullException(nameof(stateStore));
        }

        public List<BasketItem> Items => _items;

        public int TotalItemCount => _items.Sum(item => item.Quantity);

        public event EventHandler ItemsChanged;

        public async Task InitializeAsync()
        {
            _items = (await _stateStore.LoadBasketItemsAsync()).ToList();
        }

        public async Task AddItemAsync(int productId)
        {
            // How to check auth programmatically
            // https://www.youtube.com/watch?v=N8YoJRV19rw

            var existingItem = Items.Find(item => item.ProductId == productId);
            if (existingItem != null)
            {
                existingItem.Quantity += 1;
            }
            else
            {
                Items.Add(new BasketItem
                {
                    ProductId = productId,
                    Quantity = 1
                });
            }

            await _stateStore.SaveBasketItemsAsync(Items);

            OnItemsChanged(EventArgs.Empty);
        }

        private void OnItemsChanged(EventArgs e)
            => ItemsChanged?.Invoke(this, e);
    }
}
