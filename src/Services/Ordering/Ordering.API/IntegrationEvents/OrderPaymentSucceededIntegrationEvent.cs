namespace Microsoft.eShopOnContainers.Services.Ordering.API.IntegrationEvents
{
    using System;
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

    public class OrderPaymentSucceededIntegrationEvent : IntegrationEvent
    {
        public Guid OrderId { get; set; }

        public OrderPaymentSucceededIntegrationEvent()
        {
        }

        public OrderPaymentSucceededIntegrationEvent(Guid orderId) => OrderId = orderId;
    }
}