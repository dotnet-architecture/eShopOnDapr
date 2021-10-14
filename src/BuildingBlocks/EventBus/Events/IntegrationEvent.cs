using System;

namespace Microsoft.eShopOnDapr.BuildingBlocks.EventBus.Events
{
    public class IntegrationEvent
    {
        public IntegrationEvent()
        {
            Id = Guid.NewGuid();
            CreationDate = DateTime.UtcNow;
        }

        public Guid Id { get; private set; }

        public DateTime CreationDate { get; private set; }
    }

    public record IntegrationEvent2
    {
        public IntegrationEvent2()
        {
            Id = Guid.NewGuid();
            CreationDate = DateTime.UtcNow;
        }

        public Guid Id { get; }

        public DateTime CreationDate { get; }
    }
}
