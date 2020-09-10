namespace Microsoft.eShopOnContainers.Services.Catalog.API.IntegrationEvents.Events
{
    using System.Collections.Generic;
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

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