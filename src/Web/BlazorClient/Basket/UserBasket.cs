using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.eShopOnDapr.BlazorClient.Catalog;
using Microsoft.eShopOnDapr.BlazorClient.Ordering;

namespace Microsoft.eShopOnDapr.BlazorClient.Basket
{
    public class UserBasket
    {
        private readonly BasketClient _basketClient;

        public UserBasket(BasketClient basketClient)
        {
            _basketClient = basketClient
                ?? throw new ArgumentNullException(nameof(basketClient));
        }

        public List<BasketItem> Items { get; set; } = new();

        public int TotalItemCount => Items.Sum(item => item.Quantity);

        public string GetFormattedTotalPrice() => Items.Sum(
            item => item.Quantity * item.UnitPrice).ToString("0.00");

        public event EventHandler ItemsChanged;

        public async Task LoadAsync()
        {
            Items = (await _basketClient.GetItemsAsync())
                .ToList();

            OnItemsChanged(EventArgs.Empty);
        }

        public async Task AddItemAsync(CatalogItem item)
        {
            var existingItem = Items.Find(
                basketItem => basketItem.ProductId == item.Id);

            if (existingItem != null)
            {
                await SetItemQuantityAsync(existingItem, existingItem.Quantity + 1);
            }
            else
            {
                Items.Add(new BasketItem(
                    item.Id,
                    item.Name,
                    item.Price,
                    1,
                    item.PictureFileName));
            }

            await SaveItemsAsync();
        }

        public async Task RemoveItemAsync(BasketItem item)
        {
            Items.Remove(item);

            await SaveItemsAsync();
        }

        public async Task SetItemQuantityAsync(BasketItem item, int quantity)
        {
            var index = Items.IndexOf(item);
            if (index > -1 && quantity >= 1)
            {
                Items[index] = Items[index] with { Quantity = quantity }; 

                await SaveItemsAsync();
            }
        }

        public async Task CheckoutAsync(OrderForm orderForm)
        {
            var basketCheckout = new BasketCheckout(
                orderForm.Email,
                orderForm.Street,
                orderForm.City,
                orderForm.State,
                orderForm.Country,
                orderForm.CardNumber,
                orderForm.CardHolderName,
                CardExpirationDate.Parse(orderForm.CardExpirationDate),
                orderForm.CardSecurityCode);

            await _basketClient.CheckoutAsync(basketCheckout);

            // Drop basket.
            Items.Clear();
            OnItemsChanged(EventArgs.Empty);
        }

        private async Task SaveItemsAsync()
        {
            var verifiedItems = await _basketClient.SaveItemsAsync(Items);

            Items = verifiedItems.ToList();

            OnItemsChanged(EventArgs.Empty);
        }

        private void OnItemsChanged(EventArgs e)
            => ItemsChanged?.Invoke(this, e);
    }
}
