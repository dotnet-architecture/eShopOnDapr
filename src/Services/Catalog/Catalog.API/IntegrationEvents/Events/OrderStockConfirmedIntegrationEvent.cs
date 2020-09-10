namespace Microsoft.eShopOnContainers.Services.Catalog.API.IntegrationEvents.Events
{
    using BuildingBlocks.EventBus.Events;

    public class OrderStockConfirmedIntegrationEvent : IntegrationEvent
    {
        public int OrderId { get; set; }

        public OrderStockConfirmedIntegrationEvent()
        {
        }

        public OrderStockConfirmedIntegrationEvent(int orderId) => OrderId = orderId;
    }
}