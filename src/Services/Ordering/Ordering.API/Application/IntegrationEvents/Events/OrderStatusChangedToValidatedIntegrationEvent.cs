namespace Ordering.API.Application.IntegrationEvents.Events
{
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

    public class OrderStatusChangedToValidatedIntegrationEvent : IntegrationEvent
    {
        public int OrderId { get; }
        public string OrderStatus { get; }
        public string BuyerName { get; }
        public decimal Total { get; }

        public OrderStatusChangedToValidatedIntegrationEvent(int orderId, string orderStatus, string buyerName, decimal total)
        {
            OrderId = orderId;
            OrderStatus = orderStatus;
            BuyerName = buyerName;
            Total = total;
        }
    }
}