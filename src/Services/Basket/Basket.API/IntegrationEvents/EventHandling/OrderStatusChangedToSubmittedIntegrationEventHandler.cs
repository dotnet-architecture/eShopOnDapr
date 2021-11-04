namespace Microsoft.eShopOnDapr.Services.Basket.API.IntegrationEvents.EventHandling;

public class OrderStatusChangedToSubmittedIntegrationEventHandler
    : IIntegrationEventHandler<OrderStatusChangedToSubmittedIntegrationEvent>
{
    private readonly IBasketRepository _repository;

    public OrderStatusChangedToSubmittedIntegrationEventHandler(
        IBasketRepository repository)
    {
        _repository = repository;
    }

    public Task Handle(OrderStatusChangedToSubmittedIntegrationEvent @event) =>
        _repository.DeleteBasketAsync(@event.BuyerId);
}



