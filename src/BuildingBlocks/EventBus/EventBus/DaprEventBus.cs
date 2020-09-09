using System.Threading.Tasks;
using Dapr.Client;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

namespace Microsoft.eShopOnContainers.BuildingBlocks.EventBus
{
    public class DaprEventBus : IEventBus
    {
        private const string DAPR_PUBSUB_NAME = "pubsub";

        private readonly DaprClient _dapr;

        public DaprEventBus(DaprClient dapr)
        {
            _dapr = dapr;
        }

        public async Task PublishAsync<TIntegrationEvent>(TIntegrationEvent @event)
            where TIntegrationEvent : IntegrationEvent
        {
            var topicName = @event.GetType().Name;

            // We need to make sure that we pass the concrete type to PublishEventAsync,
            // which can be accomplished by casting the event to dynamic. This ensures
            // that all event fields are properly serialized.
            await _dapr.PublishEventAsync(DAPR_PUBSUB_NAME, topicName, (dynamic)@event);
        }
    }
}
