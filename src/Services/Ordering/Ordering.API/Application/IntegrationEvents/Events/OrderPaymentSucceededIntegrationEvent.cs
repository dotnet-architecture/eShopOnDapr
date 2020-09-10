namespace Ordering.API.Application.IntegrationEvents.Events
{
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

    public class OrderPaymentSucceededIntegrationEvent : IntegrationEvent
    {
        public int OrderId { get; set; }

        public OrderPaymentSucceededIntegrationEvent()
        {
        }

        public OrderPaymentSucceededIntegrationEvent(int orderId) => OrderId = orderId;
    }
}