namespace Ordering.API.Application.IntegrationEvents.Events
{
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;
    using System.Collections.Generic;

    public class OrderStockRejectedIntegrationEvent : IntegrationEvent
    {
        public int OrderId { get; set; }

        public List<ConfirmedOrderStockItem> OrderStockItems { get; set; }

        public OrderStockRejectedIntegrationEvent()
        {
        }

        public OrderStockRejectedIntegrationEvent(int orderId,
            List<ConfirmedOrderStockItem> orderStockItems)
        {
            OrderId = orderId;
            OrderStockItems = orderStockItems;
        }
    }

    public class ConfirmedOrderStockItem
    {
        public int ProductId { get; set; }
        public bool HasStock { get; set; }

        public ConfirmedOrderStockItem(int productId, bool hasStock)
        {
            ProductId = productId;
            HasStock = hasStock;
        }
    }
}