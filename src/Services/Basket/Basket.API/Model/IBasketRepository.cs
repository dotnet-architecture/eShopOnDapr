using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.Services.Basket.API.Model
{
    public interface IBasketRepository
    {
        Task<CustomerBasket> GetBasketAsync(string customerId);
        Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket);
        Task DeleteBasketAsync(string id);
    }
}
