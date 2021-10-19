using System;
using Microsoft.eShopOnDapr.BuildingBlocks.EventBus.Events;

namespace Microsoft.eShopOnDapr.Services.Catalog.API.IntegrationEvents.Events
{
    public class OrderStockConfirmedIntegrationEvent : IntegrationEvent
    {
        public Guid OrderId { get; set; }

        public OrderStockConfirmedIntegrationEvent()
        {
        }

        public OrderStockConfirmedIntegrationEvent(Guid orderId) => OrderId = orderId;
    }
}