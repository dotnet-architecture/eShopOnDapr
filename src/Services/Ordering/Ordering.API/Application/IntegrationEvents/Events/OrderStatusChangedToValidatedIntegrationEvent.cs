namespace Ordering.API.Application.IntegrationEvents.Events
{
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

    public class OrderStatusChangedToValidatedIntegrationEvent : IntegrationEvent
    {
        public int OrderId { get; set; }
        public string OrderStatus { get; set; }
        public string BuyerName { get; set; }
        public decimal Total { get; set; }

        public OrderStatusChangedToValidatedIntegrationEvent()
        {
        }

        public OrderStatusChangedToValidatedIntegrationEvent(int orderId, string orderStatus, string buyerName, decimal total)
        {
            OrderId = orderId;
            OrderStatus = orderStatus;
            BuyerName = buyerName;
            Total = total;
        }
    }
}