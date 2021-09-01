using System.Collections.Generic;

namespace eShopOnDapr.BlazorClient.Basket
{
    public record BasketData(IEnumerable<BasketItem> Items);
}
