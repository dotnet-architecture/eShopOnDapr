using Dapr.Actors;

namespace Microsoft.eShopOnDapr.Services.Basket.API.Actors
{
    public interface IBasketProcessActor : IActor
    {
        Task<CustomerBasket> GetBasketAsync();
        Task DeleteBasketAsync();

        Task UpdateBasketAsync(CustomerBasket basket);

        Task VerifyBasketAsync(IEnumerable<Product> products);
    }
}
