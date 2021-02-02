using System;
using System.Threading.Tasks;
using Basket.API.IntegrationEvents.Events;
using Dapr;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Basket.API.IntegrationEvents.EventHandling;

namespace Microsoft.eShopOnContainers.Services.Basket.API.Controllers
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

        [HttpPost("OrderStarted")]
        [Topic(DAPR_PUBSUB_NAME, "OrderStartedIntegrationEvent")]
        public async Task OrderStarted(OrderStatusChangedToSubmittedIntegrationEvent @event)
        {
            var handler = _serviceProvider.GetRequiredService<OrderStatusChangedToSubmittedIntegrationEventHandler>();
            await handler.Handle(@event);
        }
    }
}
