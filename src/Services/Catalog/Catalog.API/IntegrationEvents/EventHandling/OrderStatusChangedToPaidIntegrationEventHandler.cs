using System.Threading.Tasks;
using Microsoft.eShopOnDapr.Services.Catalog.API.Infrastructure;
using Microsoft.eShopOnDapr.BuildingBlocks.EventBus.Abstractions;
using Microsoft.eShopOnDapr.Services.Catalog.API.IntegrationEvents.Events;
using System;

namespace Microsoft.eShopOnDapr.Services.Catalog.API.IntegrationEvents.EventHandling
{
    public class OrderStatusChangedToPaidIntegrationEventHandler : 
        IIntegrationEventHandler<OrderStatusChangedToPaidIntegrationEvent>
    {
        private readonly CatalogDbContext _context;

        public OrderStatusChangedToPaidIntegrationEventHandler(CatalogDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task Handle(OrderStatusChangedToPaidIntegrationEvent @event)
        {
            //we're not blocking stock/inventory
            foreach (var orderStockItem in @event.OrderStockItems)
            {
                var catalogItem = _context.CatalogItems.Find(orderStockItem.ProductId);

                catalogItem.RemoveStock(orderStockItem.Units);
            }

            await _context.SaveChangesAsync();
        }
    }
}