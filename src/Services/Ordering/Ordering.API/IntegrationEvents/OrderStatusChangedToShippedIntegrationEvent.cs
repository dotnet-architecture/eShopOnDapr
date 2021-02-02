using System;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

namespace Microsoft.eShopOnContainers.Services.Ordering.API.IntegrationEvents
{
    public class OrderStatusChangedToShippedIntegrationEvent : IntegrationEvent
    {
        public Guid OrderId { get; set; }
        public string OrderStatus { get; set; }
        public string Description { get; set; }
        public string BuyerName { get; set; }

        public OrderStatusChangedToShippedIntegrationEvent()
        {
        }

        public OrderStatusChangedToShippedIntegrationEvent(Guid orderId, string orderStatus,
            string description, string buyerName)
        {
            OrderId = orderId;
            OrderStatus = orderStatus;
            Description = description;
            BuyerName = buyerName;
        }
    }
}
