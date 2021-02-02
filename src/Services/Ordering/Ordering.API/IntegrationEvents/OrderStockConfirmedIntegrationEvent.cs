namespace Microsoft.eShopOnContainers.Services.Ordering.API.IntegrationEvents
{
    using System;
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

    public class OrderStockConfirmedIntegrationEvent : IntegrationEvent
    {
        public Guid OrderId { get; set; }

        public OrderStockConfirmedIntegrationEvent()
        {
        }

        public OrderStockConfirmedIntegrationEvent(Guid orderId) => OrderId = orderId;
    }
}