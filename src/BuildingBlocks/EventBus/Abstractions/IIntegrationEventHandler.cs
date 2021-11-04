namespace Microsoft.eShopOnDapr.BuildingBlocks.EventBus.Abstractions;

public interface IIntegrationEventHandler<in TIntegrationEvent> : IIntegrationEventHandler
    where TIntegrationEvent : IntegrationEvent
{
    Task Handle(TIntegrationEvent @event);
}

public interface IIntegrationEventHandler
{
}
