namespace Microsoft.eShopOnContainers.Services.Ordering.API.IntegrationEvents
{
    using System;
    using System.Collections.Generic;
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

    public class OrderStatusChangedToAwaitingStockValidationIntegrationEvent : IntegrationEvent
    {
        public Guid OrderId { get; set; }
        public string OrderStatus { get; set; }
        public string Description { get; set; }
        public string BuyerName { get; set; }
        public IEnumerable<OrderStockItem> OrderStockItems { get; set; }

        public OrderStatusChangedToAwaitingStockValidationIntegrationEvent()
        {
        }

        public OrderStatusChangedToAwaitingStockValidationIntegrationEvent(Guid orderId, string orderStatus,
            string description, string buyerName, IEnumerable<OrderStockItem> orderStockItems)
        {
            OrderId = orderId;
            OrderStatus = orderStatus;
            Description = description;
            BuyerName = buyerName;
            OrderStockItems = orderStockItems;
        }
    }

    public class OrderStockItem
    {
        public int ProductId { get; set; }
        public int Units { get; set; }

        public OrderStockItem(int productId, int units)
        {
            ProductId = productId;
            Units = units;
        }
    }
}