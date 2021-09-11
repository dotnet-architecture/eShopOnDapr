using System.Threading.Tasks;
using Microsoft.eShopOnDapr.Web.Shopping.HttpAggregator.Models;

namespace Microsoft.eShopOnDapr.Web.Shopping.HttpAggregator.Services
{
    public interface IBasketService
    {
        Task UpdateAsync(BasketData currentBasket, string accessToken);
    }
}
