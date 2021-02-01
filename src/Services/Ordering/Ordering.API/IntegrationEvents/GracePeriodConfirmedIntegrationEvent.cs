namespace Microsoft.eShopOnContainers.Services.Ordering.API.IntegrationEvents
{
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

    public class GracePeriodConfirmedIntegrationEvent : IntegrationEvent
    {
        public int OrderId { get; set; }

        public GracePeriodConfirmedIntegrationEvent()
        {
        }

        public GracePeriodConfirmedIntegrationEvent(int orderId) =>
            OrderId = orderId;
    }
}
