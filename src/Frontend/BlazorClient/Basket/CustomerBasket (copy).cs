using System.Collections.Generic;

namespace eShopOnDapr.BlazorClient.Basket
{
    public class CustomerBasket2
    {
        public string BuyerId { get; set; }

        public List<BasketItem> Items { get; set; } = new();
    }
}
