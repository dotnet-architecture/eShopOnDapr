using System.Threading.Tasks;
using Microsoft.eShopOnDapr.BuildingBlocks.EventBus.Events;

namespace Microsoft.eShopOnDapr.BuildingBlocks.EventBus.Abstractions
{
    public interface IIntegrationEventHandler<in TIntegrationEvent> : IIntegrationEventHandler 
        where TIntegrationEvent: IntegrationEvent
    {
        Task Handle(TIntegrationEvent @event);
    }

    public interface IIntegrationEventHandler
    {
    }

    public interface IIntegrationEventHandler2<in TIntegrationEvent> : IIntegrationEventHandler2
    where TIntegrationEvent : IntegrationEvent2
    {
        Task Handle(TIntegrationEvent @event);
    }

    public interface IIntegrationEventHandler2
    {
    }
}
