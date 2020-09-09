using System;
using System.Threading.Tasks;
using Dapr;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.eShopOnContainers.Services.Catalog.API.IntegrationEvents.Events;
using Microsoft.eShopOnContainers.Services.Catalog.API.IntegrationEvents.EventHandling;

namespace Microsoft.eShopOnContainers.Services.Catalog.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class IntegrationEventController : ControllerBase
    {
        private const string DAPR_PUBSUB_NAME = "pubsub";

        private readonly IServiceProvider _serviceProvider;

        public IntegrationEventController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        [HttpPost("OrderStatusChangedToAwaitingStockValidation")]
        [Topic(DAPR_PUBSUB_NAME, "OrderStatusChangedToAwaitingStockValidationIntegrationEvent")]
        public async Task OrderStarted(OrderStatusChangedToAwaitingStockValidationIntegrationEvent @event)
        {
            var handler = _serviceProvider.GetRequiredService<OrderStatusChangedToAwaitingStockValidationIntegrationEventHandler>();
            await handler.Handle(@event);
        }

        [HttpPost("OrderStatusChangedToPaid")]
        [Topic(DAPR_PUBSUB_NAME, "OrderStatusChangedToPaidIntegrationEvent")]
        public async Task OrderStarted(OrderStatusChangedToPaidIntegrationEvent @event)
        {
            var handler = _serviceProvider.GetRequiredService<OrderStatusChangedToPaidIntegrationEventHandler>();
            await handler.Handle(@event);
        }
    }
}