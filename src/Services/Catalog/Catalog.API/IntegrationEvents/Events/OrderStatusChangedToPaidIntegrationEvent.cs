using System.Collections.Generic;
using Microsoft.eShopOnDapr.BuildingBlocks.EventBus.Events;

namespace Microsoft.eShopOnDapr.Services.Catalog.API.IntegrationEvents.Events
{
    public class OrderStatusChangedToPaidIntegrationEvent : IntegrationEvent
    {
        public int OrderId { get; set; }
        public IEnumerable<OrderStockItem> OrderStockItems { get; set; }

        public OrderStatusChangedToPaidIntegrationEvent()
        {
        }

        public OrderStatusChangedToPaidIntegrationEvent(int orderId,
            IEnumerable<OrderStockItem> orderStockItems)
        {
            OrderId = orderId;
            OrderStockItems = orderStockItems;
        }
    }
}