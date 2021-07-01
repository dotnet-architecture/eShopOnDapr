using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eShopOnDapr.BlazorClient.Catalog;
using Microsoft.AspNetCore.Components.Authorization;

namespace eShopOnDapr.BlazorClient.Basket
{
    public class CustomerBasket
    {
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly ApiBasketClient _apiBasketClient;
        private string _buyerId;

        public CustomerBasket(
            AuthenticationStateProvider authenticationStateProvider,
            ApiBasketClient apiBasketClient)
        {
            _authenticationStateProvider = authenticationStateProvider
                ?? throw new ArgumentNullException(nameof(authenticationStateProvider));
            _apiBasketClient = apiBasketClient
                ?? throw new ArgumentNullException(nameof(apiBasketClient));

            authenticationStateProvider.AuthenticationStateChanged += async _ => await RefreshAsync();
//                AuthenticationStateProvider_AuthenticationStateChanged;
        }

        //private void AuthenticationStateProvider_AuthenticationStateChanged(Task<AuthenticationState> task)
        //{
        //    _ = InitializeAsync();
        //}

        public List<BasketItem> Items { get; set; } = new();

        public int TotalItemCount => Items.Sum(item => item.Quantity);

        public string GetFormattedTotalPrice() => Items.Sum(
            item => item.Quantity * item.UnitPrice).ToString("0.00");

        public event EventHandler ItemsChanged;

        public async Task RefreshAsync()
        {
            Console.WriteLine("INITIALIZE!!!");

            _buyerId = await GetBuyerIdAsync();
            if (_buyerId.Length > 0)
            {
                Items = (await _apiBasketClient.LoadItemsAsync(_buyerId))
                    .ToList();

                OnItemsChanged(EventArgs.Empty);
            }
        }

        public async Task AddItemAsync(CatalogItem item)
        {
            if (_buyerId.Length > 0)
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

                await _apiBasketClient.SaveItemsAsync(_buyerId, Items);

                OnItemsChanged(EventArgs.Empty);
            }
        }

        public async Task RemoveItemAsync(BasketItem item)
        {
            if (_buyerId.Length > 0)
            {
                Items.Remove(item);

                await _apiBasketClient.SaveItemsAsync(_buyerId, Items);

                OnItemsChanged(EventArgs.Empty);
            }
        }

        public async Task SetItemQuantityAsync(BasketItem item, int quantity)
        {
            if (_buyerId.Length > 0 && Items.Contains(item) && quantity >= 1)
            {
                item.Quantity = quantity;

                await _apiBasketClient.SaveItemsAsync(_buyerId, Items);

                OnItemsChanged(EventArgs.Empty);
            }
        }

        private async Task<string> GetBuyerIdAsync()
        {
            var authenticationState =
                await _authenticationStateProvider.GetAuthenticationStateAsync();

            if (authenticationState.User.Identity.IsAuthenticated)
            {
                var subjectClaim = authenticationState.User.Claims.FirstOrDefault(
                    claim => claim.Type == "sub");
                if (subjectClaim != null)
                {
                    return subjectClaim.Value;
                }
            }

            return string.Empty;
        }

        private void OnItemsChanged(EventArgs e)
            => ItemsChanged?.Invoke(this, e);
    }
}
