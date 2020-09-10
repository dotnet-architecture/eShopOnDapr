namespace Payment.API.IntegrationEvents.Events
{
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

    public class OrderPaymentFailedIntegrationEvent : IntegrationEvent
    {
        public int OrderId { get; set; }

        public OrderPaymentFailedIntegrationEvent()
        {
        }

        public OrderPaymentFailedIntegrationEvent(int orderId) => OrderId = orderId;
    }
}