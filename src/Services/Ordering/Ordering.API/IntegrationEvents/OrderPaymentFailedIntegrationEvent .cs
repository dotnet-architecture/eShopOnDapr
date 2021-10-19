using System;
using Microsoft.eShopOnDapr.BuildingBlocks.EventBus.Events;

namespace Microsoft.eShopOnDapr.Services.Ordering.API.IntegrationEvents
{
    public class OrderPaymentFailedIntegrationEvent : IntegrationEvent
    {
        public Guid OrderId { get; set; }

        public OrderPaymentFailedIntegrationEvent ()
        {
        }

        public OrderPaymentFailedIntegrationEvent(Guid orderId) => OrderId = orderId;
    }
}