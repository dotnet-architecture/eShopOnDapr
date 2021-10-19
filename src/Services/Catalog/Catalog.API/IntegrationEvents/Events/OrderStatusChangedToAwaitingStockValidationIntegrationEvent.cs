using System;
using System.Collections.Generic;
using Microsoft.eShopOnDapr.BuildingBlocks.EventBus.Events;

namespace Microsoft.eShopOnDapr.Services.Catalog.API.IntegrationEvents.Events
{
    public class OrderStatusChangedToAwaitingStockValidationIntegrationEvent : IntegrationEvent
    {
        public Guid OrderId { get; set; }
        public IEnumerable<OrderStockItem> OrderStockItems { get; set; }

        public OrderStatusChangedToAwaitingStockValidationIntegrationEvent()
        {
        }

        public OrderStatusChangedToAwaitingStockValidationIntegrationEvent(Guid orderId,
            IEnumerable<OrderStockItem> orderStockItems)
        {
            OrderId = orderId;
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