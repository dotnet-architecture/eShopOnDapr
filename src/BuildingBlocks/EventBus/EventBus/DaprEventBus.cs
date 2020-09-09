using System.Threading.Tasks;
using Dapr.Client;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

namespace Microsoft.eShopOnContainers.BuildingBlocks.EventBus
{
    public class DaprEventBus : IEventBus
    {
        private readonly DaprClient _dapr;

        public DaprEventBus(DaprClient dapr)
        {
            _dapr = dapr;
        }

        public async Task PublishAsync<TIntegrationEvent>(TIntegrationEvent @event)
            where TIntegrationEvent : IntegrationEvent
        {
            await _dapr.PublishEventAsync(@event.GetType().Name, (dynamic)@event);
        }
    }
}
