namespace Microsoft.eShopOnContainers.Services.Ordering.API.IntegrationEvents
{
    using System;
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

    public class OrderPaymentFailedIntegrationEvent : IntegrationEvent
    {
        public Guid OrderId { get; set; }

        public OrderPaymentFailedIntegrationEvent ()
        {
        }

        public OrderPaymentFailedIntegrationEvent(Guid orderId) => OrderId = orderId;
    }
}