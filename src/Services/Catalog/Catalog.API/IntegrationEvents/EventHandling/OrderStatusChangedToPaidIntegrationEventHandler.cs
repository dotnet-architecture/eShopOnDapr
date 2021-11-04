namespace Microsoft.eShopOnDapr.Services.Catalog.API.IntegrationEvents.EventHandling;

public class OrderStatusChangedToPaidIntegrationEventHandler : 
    IIntegrationEventHandler<OrderStatusChangedToPaidIntegrationEvent>
{
    private readonly CatalogDbContext _context;

    public OrderStatusChangedToPaidIntegrationEventHandler(CatalogDbContext context)
    {
        _context = context;
    }

    public async Task Handle(OrderStatusChangedToPaidIntegrationEvent @event)
    {
        //we're not blocking stock/inventory
        foreach (var orderStockItem in @event.OrderStockItems)
        {
            var catalogItem = _context.CatalogItems.Find(orderStockItem.ProductId);
            if (catalogItem != null)
            {
                catalogItem.RemoveStock(orderStockItem.Units);
            }
        }

        await _context.SaveChangesAsync();
    }
}
