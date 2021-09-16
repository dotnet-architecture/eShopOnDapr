using System;
using System.Collections.Generic;
using Microsoft.eShopOnDapr.BuildingBlocks.EventBus.Events;

namespace Microsoft.eShopOnDapr.Services.Ordering.API.IntegrationEvents
{
    public class OrderStatusChangedToAwaitingStockValidationIntegrationEvent : IntegrationEvent
    {
        public Guid OrderId { get; set; }
        public string OrderStatus { get; set; }
        public string Description { get; set; }
        public IEnumerable<OrderStockItem> OrderStockItems { get; set; }
        public string BuyerId { get; set; }


        public OrderStatusChangedToAwaitingStockValidationIntegrationEvent()
        {
        }

        public OrderStatusChangedToAwaitingStockValidationIntegrationEvent(Guid orderId, string orderStatus,
            string description, IEnumerable<OrderStockItem> orderStockItems, string buyerId)
        {
            OrderId = orderId;
            OrderStatus = orderStatus;
            Description = description;
            OrderStockItems = orderStockItems;
            BuyerId = buyerId;
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