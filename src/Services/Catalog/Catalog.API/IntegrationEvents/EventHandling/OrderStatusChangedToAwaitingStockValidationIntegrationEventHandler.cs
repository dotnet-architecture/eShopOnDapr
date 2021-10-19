using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.eShopOnDapr.BuildingBlocks.EventBus.Abstractions;
using Microsoft.eShopOnDapr.BuildingBlocks.EventBus.Events;
using Microsoft.eShopOnDapr.Services.Catalog.API.Infrastructure;
using Microsoft.eShopOnDapr.Services.Catalog.API.IntegrationEvents.Events;
using Microsoft.Extensions.Logging;

namespace Microsoft.eShopOnDapr.Services.Catalog.API.IntegrationEvents.EventHandling
{
    public class OrderStatusChangedToAwaitingStockValidationIntegrationEventHandler : 
        IIntegrationEventHandler<OrderStatusChangedToAwaitingStockValidationIntegrationEvent>
    {
        private readonly CatalogDbContext _context;
        private readonly IEventBus _eventBus;
        private readonly ILogger _logger;

        public OrderStatusChangedToAwaitingStockValidationIntegrationEventHandler(
            CatalogDbContext context,
            IEventBus eventBus,
            ILogger<OrderStatusChangedToAwaitingStockValidationIntegrationEventHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(OrderStatusChangedToAwaitingStockValidationIntegrationEvent @event)
        {
            var confirmedOrderStockItems = new List<ConfirmedOrderStockItem>();

            foreach (var orderStockItem in @event.OrderStockItems)
            {
                var catalogItem = _context.CatalogItems.Find(orderStockItem.ProductId);
                var hasStock = catalogItem.AvailableStock >= orderStockItem.Units;
                var confirmedOrderStockItem = new ConfirmedOrderStockItem(catalogItem.Id, hasStock);

                confirmedOrderStockItems.Add(confirmedOrderStockItem);
            }

            // Simulate work
            await Task.Delay(3000);

            var confirmedIntegrationEvent = confirmedOrderStockItems.Any(c => !c.HasStock)
                ? (IntegrationEvent)new OrderStockRejectedIntegrationEvent(@event.OrderId, confirmedOrderStockItems)
                : new OrderStockConfirmedIntegrationEvent(@event.OrderId);

            await _eventBus.PublishAsync(confirmedIntegrationEvent);
        }
    }
}