namespace Microsoft.eShopOnDapr.Services.Basket.API.IntegrationEvents.EventHandling;

public class VerifiedProductIntegrationEventHandler
    : IIntegrationEventHandler<VerifiedBasketProductIntegrationEvent>
{
    private readonly IBasketRepository _repository;

    public VerifiedProductIntegrationEventHandler(
        IBasketRepository repository)
    {
        _repository = repository;
    }

    public Task HandleAsync(VerifiedBasketProductIntegrationEvent @event) =>
        _repository.VerifyBasketProductsAsync(@event.BuyerId, @event.Products);
}



