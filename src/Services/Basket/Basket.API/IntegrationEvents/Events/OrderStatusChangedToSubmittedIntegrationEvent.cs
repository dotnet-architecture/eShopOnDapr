using System;
using Microsoft.eShopOnDapr.BuildingBlocks.EventBus.Events;

namespace Microsoft.eShopOnDapr.Services.Basket.API.IntegrationEvents.Events
{
    public class OrderStatusChangedToSubmittedIntegrationEvent : IntegrationEvent
    {
        public Guid OrderId { get; set; }
        public string OrderStatus { get; set; }
        public string BuyerId { get; set; }
    }
}
