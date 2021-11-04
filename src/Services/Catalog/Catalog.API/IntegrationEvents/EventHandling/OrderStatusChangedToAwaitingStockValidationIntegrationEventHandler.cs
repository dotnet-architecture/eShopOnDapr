namespace Microsoft.eShopOnDapr.Services.Catalog.API.IntegrationEvents.EventHandling;

public class OrderStatusChangedToAwaitingStockValidationIntegrationEventHandler : 
    IIntegrationEventHandler<OrderStatusChangedToAwaitingStockValidationIntegrationEvent>
{
    private readonly CatalogDbContext _context;
    private readonly IEventBus _eventBus;

    public OrderStatusChangedToAwaitingStockValidationIntegrationEventHandler(
        CatalogDbContext context,
        IEventBus eventBus)
    {
        _context = context;
        _eventBus = eventBus;
    }

    public async Task Handle(OrderStatusChangedToAwaitingStockValidationIntegrationEvent @event)
    {
        var confirmedOrderStockItems = new List<ConfirmedOrderStockItem>();

        foreach (var orderStockItem in @event.OrderStockItems)
        {
            var catalogItem = _context.CatalogItems.Find(orderStockItem.ProductId);
            if (catalogItem != null)
            {
                var hasStock = catalogItem.AvailableStock >= orderStockItem.Units;
                var confirmedOrderStockItem = new ConfirmedOrderStockItem(catalogItem.Id, hasStock);

                confirmedOrderStockItems.Add(confirmedOrderStockItem);
            }
        }

        // Simulate work
        await Task.Delay(3000);

        var confirmedIntegrationEvent = confirmedOrderStockItems.Any(c => !c.HasStock)
            ? (IntegrationEvent)new OrderStockRejectedIntegrationEvent(@event.OrderId, confirmedOrderStockItems)
            : new OrderStockConfirmedIntegrationEvent(@event.OrderId);

        await _eventBus.PublishAsync(confirmedIntegrationEvent);
    }
}
