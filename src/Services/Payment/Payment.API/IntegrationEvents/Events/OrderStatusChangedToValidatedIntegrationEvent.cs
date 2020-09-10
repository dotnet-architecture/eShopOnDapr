namespace Payment.API.IntegrationEvents.Events
{
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;
    using Newtonsoft.Json;

    public class OrderStatusChangedToValidatedIntegrationEvent : IntegrationEvent
    {
        public int OrderId { get; set; }

        public decimal Total { get; set; }

        public OrderStatusChangedToValidatedIntegrationEvent()
        {
        }
    }
}