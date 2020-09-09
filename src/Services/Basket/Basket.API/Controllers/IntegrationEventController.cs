using System;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Basket.API.IntegrationEvents.Events;
using Dapr;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<IntegrationEventController> _logger;

        public IntegrationEventController(IServiceProvider serviceProvider, ILogger<IntegrationEventController> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        [HttpPost("OrderStarted")]
        [Topic(DAPR_PUBSUB_NAME, "OrderStartedIntegrationEvent")]
        public async Task OrderStarted(OrderStartedIntegrationEvent @event)
        {
            var handler = _serviceProvider.GetRequiredService<OrderStartedIntegrationEventHandler>();
            await handler.Handle(@event);
        }
    }
}
