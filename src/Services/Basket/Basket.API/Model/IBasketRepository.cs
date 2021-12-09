namespace Microsoft.eShopOnDapr.Services.Basket.API.Model;

public interface IBasketRepository
{
    Task<CustomerBasket> GetBasketAsync(string buyerId);
    Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket);
    Task VerifyBasketProductsAsync(string buyerId, IEnumerable<Product> products);

    Task DeleteBasketAsync(string buyerId);
}
