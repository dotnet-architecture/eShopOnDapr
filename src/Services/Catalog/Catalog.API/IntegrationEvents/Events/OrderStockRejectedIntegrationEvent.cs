﻿namespace Microsoft.eShopOnContainers.Services.Catalog.API.IntegrationEvents.Events
{
    using BuildingBlocks.EventBus.Events;
    using System;
    using System.Collections.Generic;

    public class OrderStockRejectedIntegrationEvent : IntegrationEvent
    {
        public Guid OrderId { get; set; }

        public List<ConfirmedOrderStockItem> OrderStockItems { get; set; }

        public OrderStockRejectedIntegrationEvent()
        {
        }

        public OrderStockRejectedIntegrationEvent(Guid orderId,
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