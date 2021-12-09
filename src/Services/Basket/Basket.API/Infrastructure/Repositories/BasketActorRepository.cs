using Dapr.Actors;
using Dapr.Actors.Client;
using Microsoft.eShopOnDapr.Services.Basket.API.Actors;

namespace Microsoft.eShopOnDapr.Services.Basket.API.Infrastructure.Repositories;

public class BasketActorRepository : IBasketRepository
{
    private const string StoreName = "eshop-statestore";

    private readonly IActorProxyFactory _actorProxyFactory;
    private readonly ILogger _logger;

    public BasketActorRepository(IActorProxyFactory actorProxyFactory, ILogger<BasketStateRepository> logger)
    {
        _actorProxyFactory = actorProxyFactory;
        _logger = logger;
    }

    public Task DeleteBasketAsync(string buyerId) =>
        GetBasketProcessActor(buyerId).DeleteBasketAsync();

    public Task<CustomerBasket> GetBasketAsync(string buyerId) =>
        GetBasketProcessActor(buyerId).GetBasketAsync();

    public async Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket)
    {
        var actor = GetBasketProcessActor(basket.BuyerId);
        await actor.UpdateBasketAsync(basket);

        _logger.LogInformation("Basket item persisted successfully.");

        return await actor.GetBasketAsync();
    }

    public Task VerifyBasketProductsAsync(string buyerId, IEnumerable<Product> products) =>
        GetBasketProcessActor(buyerId).VerifyBasketAsync(products);

    private IBasketProcessActor GetBasketProcessActor(string buyerId)
    {
        var actorId = new ActorId(buyerId);
        return _actorProxyFactory.CreateActorProxy<IBasketProcessActor>(
            actorId,
            nameof(BasketProcessActor));
    }
}
