using System;
using Microsoft.eShopOnDapr.BuildingBlocks.EventBus.Events;

namespace Microsoft.eShopOnDapr.Services.Payment.API.IntegrationEvents.Events
{
    public class OrderPaymentSucceededIntegrationEvent : IntegrationEvent
    {
        public Guid OrderId { get; set; }

        public OrderPaymentSucceededIntegrationEvent()
        {
        }

        public OrderPaymentSucceededIntegrationEvent(Guid orderId) => OrderId = orderId;
    }
}