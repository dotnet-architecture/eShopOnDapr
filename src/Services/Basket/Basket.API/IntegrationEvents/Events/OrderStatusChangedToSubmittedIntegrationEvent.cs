using System;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

namespace Basket.API.IntegrationEvents.Events
{
    public class OrderStatusChangedToSubmittedIntegrationEvent : IntegrationEvent
    {
        public Guid OrderId { get; set; }
        public string OrderStatus { get; set; }
        public string BuyerId { get; set; }
        public string BuyerName { get; set; }
    }
}
