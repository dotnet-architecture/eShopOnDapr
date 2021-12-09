namespace Microsoft.eShopOnDapr.Services.Basket.API.Actors;

public class BasketProcessActor : Actor, IBasketProcessActor
{
    private const string BasketStateStorage = nameof(BasketStateStorage);
    private const string BasketStatusStorage = nameof(BasketStatusStorage);

    private const string BasketSubmittedReminder = nameof(BasketSubmittedReminder);
    private const string BasketProductConfirmedReminder = nameof(BasketProductConfirmedReminder);
    private const string BasketProductRejectedReminder = nameof(BasketProductRejectedReminder);

    private readonly IEventBus _eventBus;

    public BasketProcessActor(ActorHost host, IEventBus eventBus) : base(host)
    {
        _eventBus = eventBus;
    }

    private string BasketActorId => Id.GetId();

    public Task<CustomerBasket> GetBasketAsync()
    {
        return StateManager.GetStateAsync<CustomerBasket>(BasketStateStorage);
    }

    public async Task DeleteBasketAsync()
    {
        await StateManager.RemoveStateAsync(BasketStateStorage);
        await StateManager.RemoveStateAsync(BasketStatusStorage);
    }

    public async Task UpdateBasketAsync(CustomerBasket basket)
    {
        await StateManager.SetStateAsync(BasketStateStorage, basket);
        await StateManager.SetStateAsync(BasketStatusStorage, BasketStatus.AwaitingProductValidation);

        await _eventBus.PublishAsync(new VerifyProductIntegrationEvent(BasketActorId, basket.Items.Select(i => i.ProductId).ToArray()));
    }

    public async Task VerifyBasketAsync(IEnumerable<Product> products)
    {
        var statusChanged = await TryUpdateStatusAsync(BasketStatus.AwaitingProductValidation, BasketStatus.Validated);
        if (statusChanged)
        {
            var basket = await StateManager.GetStateAsync<CustomerBasket>(BasketStateStorage);

            var verifiedBasket = VerifyProducts(basket, products);

            await StateManager.SetStateAsync(BasketStateStorage, verifiedBasket);

            if(verifiedBasket.IsVerified)
            { 
                await _eventBus.PublishAsync(new BasketValidatedIntegrationEvent(
                    BasketActorId,
                    "All the products were confirmed as valid.",
                    basket.GetTotal(),
                    basket.BuyerId));
            }
        }
    }

    private async Task<bool> TryUpdateStatusAsync(BasketStatus expectedStatus, BasketStatus newStatus)
    {
        var basketStatus = await StateManager.TryGetStateAsync<BasketStatus>(BasketStatusStorage);
        if (!basketStatus.HasValue)
        {
            Logger.LogWarning("Basket with Id: {BasketActorId} cannot be updated because it doesn't exist",
                BasketActorId);

            return false;
        }

        if (basketStatus.Value.Id != expectedStatus.Id)
        {
            Logger.LogWarning("Basket with Id: {BasketActorId} is in status {Status} instead of expected status {ExpectedStatus}",
                BasketActorId, basketStatus.Value.Name, expectedStatus.Name);

            return false;
        }

        await StateManager.SetStateAsync(BasketStatusStorage, newStatus);

        return true;
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
