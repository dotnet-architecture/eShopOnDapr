using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eShopOnDapr.BlazorClient.Catalog;
using eShopOnDapr.BlazorClient.Ordering;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace eShopOnDapr.BlazorClient.Basket
{
    // TODO Rename to UserBasket, remove auth state stuff
    public class CustomerBasket
    {
        private readonly UserInfo _userInfo;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly ApiBasketClient _apiBasketClient;
        private string _buyerId;

        public CustomerBasket(
            UserInfo userInfo,
            AuthenticationStateProvider authenticationStateProvider,
            ApiBasketClient apiBasketClient)
        {
            _userInfo = userInfo ?? throw new ArgumentNullException(nameof(UserInfo));
            _authenticationStateProvider = authenticationStateProvider
                ?? throw new ArgumentNullException(nameof(authenticationStateProvider));
            _apiBasketClient = apiBasketClient
                ?? throw new ArgumentNullException(nameof(apiBasketClient));

            // TODO Do this in global auth thingie
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
                Console.WriteLine("BuyerId: " + _buyerId);

                try
                {
                    Items = (await _apiBasketClient.LoadItemsAsync(_buyerId))
                        .ToList();

                    OnItemsChanged(EventArgs.Empty);
                }
                catch (AccessTokenNotAvailableException)
                {
                    Console.WriteLine("AccessTokenNotAvailableException");
                }
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

        public Task CheckoutAsync(OrderForm orderForm)
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

            return _apiBasketClient.CheckoutAsync(basketCheckout);
        }

        private async Task<string> GetBuyerIdAsync()
        {
            return _userInfo.Id ?? string.Empty;

            //var authenticationState =
            //    await _authenticationStateProvider.GetAuthenticationStateAsync();

            //if (authenticationState.User.Identity.IsAuthenticated)
            //{
            //    var subjectClaim = authenticationState.User.Claims.FirstOrDefault(
            //        claim => claim.Type == "sub");
            //    if (subjectClaim != null)
            //    {
            //        return subjectClaim.Value;
            //    }
            //}

            //return string.Empty;
        }

        private void OnItemsChanged(EventArgs e)
            => ItemsChanged?.Invoke(this, e);
    }
}
