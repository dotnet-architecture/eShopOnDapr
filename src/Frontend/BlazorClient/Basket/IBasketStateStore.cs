using System.Collections.Generic;
using System.Threading.Tasks;

namespace eShopOnDapr.BlazorClient.Basket
{
    public interface IBasketStateStore
    {
        Task<IEnumerable<BasketItem>> LoadBasketItemsAsync();

        Task SaveBasketItemsAsync(IEnumerable<BasketItem> items);
    }
}