using System.Collections.Generic;

namespace Microsoft.eShopOnDapr.BlazorClient.Basket
{
    public record BasketData(IEnumerable<BasketItem> Items);
}
