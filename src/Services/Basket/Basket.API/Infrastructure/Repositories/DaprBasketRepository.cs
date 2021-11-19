namespace Microsoft.eShopOnDapr.Services.Basket.API.Infrastructure.Repositories;

public class DaprBasketRepository : IBasketRepository
{
    private const string StoreName = "eshop-statestore";

    private readonly DaprClient _daprClient;
    private readonly ILogger _logger;

    public DaprBasketRepository(DaprClient daprClient, ILogger<DaprBasketRepository> logger)
    {
        _daprClient = daprClient;
        _logger = logger;
    }

    public Task DeleteBasketAsync(string id) =>
        _daprClient.DeleteStateAsync(StoreName, id);

    public Task<CustomerBasket> GetBasketAsync(string customerId) =>
        _daprClient.GetStateAsync<CustomerBasket>(StoreName, customerId);

    public async Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket)
    {
        var state = await _daprClient.GetStateEntryAsync<CustomerBasket>(StoreName, basket.BuyerId);
        state.Value = basket;

        await state.SaveAsync();

        _logger.LogInformation("Basket item persisted successfully.");

        return await GetBasketAsync(basket.BuyerId);
    }
}
