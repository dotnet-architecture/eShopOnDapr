using System;
using Microsoft.eShopOnDapr.BuildingBlocks.EventBus.Events;

namespace Microsoft.eShopOnDapr.Services.Ordering.API.IntegrationEvents
{
    public class OrderStatusChangedToValidatedIntegrationEvent : IntegrationEvent
    {
        public Guid OrderId { get; set; }
        public string OrderStatus { get; set; }
        public string Description { get; set; }
        public decimal Total { get; set; }
        public string BuyerId { get; set; }

        public OrderStatusChangedToValidatedIntegrationEvent()
        {
        }

        public OrderStatusChangedToValidatedIntegrationEvent(Guid orderId, string orderStatus,
            string description, decimal total, string buyerId)
        {
            OrderId = orderId;
            OrderStatus = orderStatus;
            Description = description;
            Total = total;
            BuyerId = buyerId;
        }
    }
}