namespace Ordering.API.Application.IntegrationEvents.Events
{
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;
    using Newtonsoft.Json;

    public class OrderCouponRejectedIntegrationEvent : IntegrationEvent
    {
        [JsonProperty]
        public int OrderId { get; private set; }

        [JsonProperty]
        public string Code { get; private set; }
    }
}