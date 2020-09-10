namespace Ordering.API.Application.IntegrationEvents.Events
{
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

    public class OrderStockConfirmedIntegrationEvent : IntegrationEvent
    {
        public int OrderId { get; set; }

        public OrderStockConfirmedIntegrationEvent()
        {
        }

        public OrderStockConfirmedIntegrationEvent(int orderId) => OrderId = orderId;
    }
}