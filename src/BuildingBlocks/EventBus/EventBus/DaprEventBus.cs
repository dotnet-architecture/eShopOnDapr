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

            await _dapr.PublishEventAsync<TIntegrationEvent>(DAPR_PUBSUB_NAME, topicName, @event);
        }
    }
}
