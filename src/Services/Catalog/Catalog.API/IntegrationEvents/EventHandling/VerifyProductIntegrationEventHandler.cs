namespace Microsoft.eShopOnDapr.Services.Catalog.API.IntegrationEvents.EventHandling;

public class VerifyProductIntegrationEventHandler : 
    IIntegrationEventHandler<VerifyProductIntegrationEvent>
{
    private readonly ICatalogReader _catalogReader;
    private readonly IEventBus _eventBus;

    public VerifyProductIntegrationEventHandler(
        ICatalogReader catalogReader,
        IEventBus eventBus)
    {
        _catalogReader = catalogReader;
        _eventBus = eventBus;
    }

    public async Task HandleAsync(VerifyProductIntegrationEvent @event)
    {
        var items = await _catalogReader.ReadAsync(@event.ProductIds);

        var verifiedIntegrationEvent = new VerifiedBasketProductIntegrationEvent(@event.BuyerId, items.Select(i => new Product(i.Id, i.Name, i.Price, i.PictureFileName)).ToArray());

        await _eventBus.PublishAsync(verifiedIntegrationEvent);
    }
}
