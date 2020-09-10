using System;
using System.Threading.Tasks;
using Dapr;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Payment.API.IntegrationEvents.EventHandling;
using Payment.API.IntegrationEvents.Events;

namespace Payment.API.Controllers
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

        [HttpPost("OrderStatusChangedToValidated")]
        [Topic(DAPR_PUBSUB_NAME, "OrderStatusChangedToValidatedIntegrationEvent")]
        public async Task OrderStarted(OrderStatusChangedToValidatedIntegrationEvent @event)
        {
            var handler = _serviceProvider.GetRequiredService<OrderStatusChangedToValidatedIntegrationEventHandler>();
            await handler.Handle(@event);
        }
    }
}