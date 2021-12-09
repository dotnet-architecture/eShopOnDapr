namespace Microsoft.eShopOnDapr.Services.Basket.API.Infrastructure.Repositories;

public class BasketStateRepository : IBasketRepository
{
    private const string StoreName = "eshop-statestore";

    private readonly DaprClient _daprClient;
    private readonly ILogger _logger;

    public BasketStateRepository(DaprClient daprClient, ILogger<BasketStateRepository> logger)
    {
        _daprClient = daprClient;
        _logger = logger;
    }

    public Task DeleteBasketAsync(string buyerId) =>
        _daprClient.DeleteStateAsync(StoreName, buyerId);

    public Task<CustomerBasket> GetBasketAsync(string buyerId) =>
        _daprClient.GetStateAsync<CustomerBasket>(StoreName, buyerId);

    public async Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket)
    {
        var state = await _daprClient.GetStateEntryAsync<CustomerBasket>(StoreName, basket.BuyerId);
        state.Value = basket;

        await state.SaveAsync();

        _logger.LogInformation("Basket item persisted successfully.");

        return await GetBasketAsync(basket.BuyerId);
    }

    public async Task VerifyBasketProductsAsync(string buyerId, IEnumerable<Product> products)
    {
        var state = await _daprClient.GetStateEntryAsync<CustomerBasket>(StoreName, buyerId);

        var verifiedBasket = VerifyProducts(state.Value, products);

        state.Value = verifiedBasket;

        await state.SaveAsync();
    }

    private static CustomerBasket VerifyProducts(CustomerBasket basket, IEnumerable<Product> products)
    {
        foreach (var item in basket.Items)
        {
            var product = products.SingleOrDefault(ci => ci.Id == item.ProductId);
            if (product is not null)
            {
                item.ProductName = product.Name;
                item.UnitPrice = product.Price;
                item.PictureFileName = product.PictureFileName;
                item.IsVerified = true;
            }
        }

        return basket;
    }
}
