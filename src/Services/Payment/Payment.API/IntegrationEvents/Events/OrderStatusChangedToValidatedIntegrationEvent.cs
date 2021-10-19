using System;
using Microsoft.eShopOnDapr.BuildingBlocks.EventBus.Events;

namespace Microsoft.eShopOnDapr.Services.Payment.API.IntegrationEvents.Events
{
    public class OrderStatusChangedToValidatedIntegrationEvent : IntegrationEvent
    {
        public Guid OrderId { get; set; }

        public decimal Total { get; set; }

        public OrderStatusChangedToValidatedIntegrationEvent()
        {
        }
    }
}