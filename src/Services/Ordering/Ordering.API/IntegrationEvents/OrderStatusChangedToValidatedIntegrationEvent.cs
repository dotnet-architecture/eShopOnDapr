namespace Microsoft.eShopOnContainers.Services.Ordering.API.IntegrationEvents
{
    using System;
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

    public class OrderStatusChangedToValidatedIntegrationEvent : IntegrationEvent
    {
        public Guid OrderId { get; set; }
        public string OrderStatus { get; set; }
        public string Description { get; set; }
        public string BuyerName { get; set; }
        public decimal Total { get; set; }

        public OrderStatusChangedToValidatedIntegrationEvent()
        {
        }

        public OrderStatusChangedToValidatedIntegrationEvent(Guid orderId, string orderStatus,
            string description, string buyerName, decimal total)
        {
            OrderId = orderId;
            OrderStatus = orderStatus;
            Description = description;
            BuyerName = buyerName;
            Total = total;
        }
    }
}