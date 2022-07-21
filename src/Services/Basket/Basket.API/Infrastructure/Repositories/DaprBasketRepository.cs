namespace Microsoft.eShopOnDapr.Services.Basket.API.Infrastructure.Repositories;

public class DaprBasketRepository : IBasketRepository
{
    private readonly DaprClient _daprClient;
    private readonly BasketSettings _settings;
    private readonly ILogger _logger;

    public DaprBasketRepository(
        DaprClient daprClient,
        IOptions<BasketSettings> options,
        ILogger<DaprBasketRepository> logger)
    {
        _daprClient = daprClient;
        _settings = options.Value;
        _logger = logger;
    }

    public Task DeleteBasketAsync(string id) =>
        _daprClient.DeleteStateAsync(_settings.StateStoreComponentName, id);

    public Task<CustomerBasket> GetBasketAsync(string customerId) =>
        _daprClient.GetStateAsync<CustomerBasket>(_settings.StateStoreComponentName, customerId);

    public async Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket)
    {
        var state = await _daprClient.GetStateEntryAsync<CustomerBasket>(_settings.StateStoreComponentName, basket.BuyerId);
        state.Value = basket;

        await state.SaveAsync();

        _logger.LogInformation("Basket item persisted successfully.");

        return await GetBasketAsync(basket.BuyerId);
    }
}
