using System;
using Newtonsoft.Json;

namespace Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events
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
}
