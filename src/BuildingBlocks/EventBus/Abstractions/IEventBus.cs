namespace Microsoft.eShopOnDapr.BuildingBlocks.EventBus.Abstractions;

public interface IEventBus
{
    Task PublishAsync<TIntegrationEvent>(TIntegrationEvent @event)
        where TIntegrationEvent : IntegrationEvent;
}
