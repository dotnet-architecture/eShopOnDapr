using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eShopOnDapr.BlazorClient.Catalog;
using eShopOnDapr.BlazorClient.Ordering;

namespace eShopOnDapr.BlazorClient.Basket
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
                existingItem.Quantity += 1;
            }
            else
            {
                Items.Add(new BasketItem
                {
                    ProductId = item.Id,
                    ProductName = item.Name,
                    Quantity = 1,
                    PictureUrl = item.PictureUri,
                    UnitPrice = item.Price
                });
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
            if (Items.Contains(item) && quantity >= 1)
            {
                item.Quantity = quantity;

                await SaveItemsAsync();
            }
        }

        public async Task CheckoutAsync(OrderForm orderForm)
        {
            var basketCheckout = new BasketCheckout
            {
                UserEmail = orderForm.Email,
                Street = orderForm.Street,
                City = orderForm.City,
                State = orderForm.State,
                Country = orderForm.Country,
                CardNumber = orderForm.CardNumber,
                CardHolderName = orderForm.CardHolderName,
                CardExpirationDate = CardExpirationDate.Parse(orderForm.CardExpirationDate),
                CardSecurityCode = orderForm.CardSecurityCode
            };

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
