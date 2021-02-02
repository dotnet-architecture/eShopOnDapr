using System;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

namespace Microsoft.eShopOnContainers.Services.Ordering.API.IntegrationEvents
{
    public class OrderStatusChangedToSubmittedIntegrationEvent : IntegrationEvent
    {
        public Guid OrderId { get; set; }
        public string OrderStatus { get; set; }
        public string BuyerId { get; set; }
        public string BuyerName { get; set; }

        public OrderStatusChangedToSubmittedIntegrationEvent()
        {
        }

        public OrderStatusChangedToSubmittedIntegrationEvent(Guid orderId, string orderStatus,
            string buyerId, string buyerName)
        {
            OrderId = orderId;
            OrderStatus = orderStatus;
            BuyerId = buyerId;
            BuyerName = buyerName;
        }
    }
}
