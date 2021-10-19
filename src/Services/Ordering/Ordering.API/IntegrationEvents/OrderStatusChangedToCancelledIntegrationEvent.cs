using System;
using Microsoft.eShopOnDapr.BuildingBlocks.EventBus.Events;

namespace Microsoft.eShopOnDapr.Services.Ordering.API.IntegrationEvents
{
    public class OrderStatusChangedToCancelledIntegrationEvent : IntegrationEvent
    {
        public Guid OrderId { get; set; }
        public string OrderStatus { get; set; }
        public string Description { get; set; }
        public string BuyerId { get; set; }

        public OrderStatusChangedToCancelledIntegrationEvent()
        {
        }

        public OrderStatusChangedToCancelledIntegrationEvent(Guid orderId, string orderStatus,
            string description, string buyerId)
        {
            OrderId = orderId;
            OrderStatus = orderStatus;
            Description = description;
            BuyerId = buyerId;
        }
    }
}
