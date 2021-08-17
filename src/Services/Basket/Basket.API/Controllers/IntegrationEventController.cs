using System.Threading.Tasks;
using Basket.API.IntegrationEvents.Events;
using Dapr;
using Microsoft.AspNetCore.Mvc;
using Basket.API.IntegrationEvents.EventHandling;
using System;

namespace Microsoft.eShopOnContainers.Services.Basket.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class IntegrationEventController : ControllerBase
    {
        private const string DAPR_PUBSUB_NAME = "pubsub";

        [HttpPost("OrderStatusChangedToSubmitted")]
        [Topic(DAPR_PUBSUB_NAME, "OrderStatusChangedToSubmittedIntegrationEvent")]
        public Task HandleAsync(
            OrderStatusChangedToSubmittedIntegrationEvent @event,
            [FromServices] OrderStatusChangedToSubmittedIntegrationEventHandler handler)
        {
            if (@event == null) throw new ArgumentNullException(nameof(@event));
            if (handler == null) throw new ArgumentNullException(nameof(handler));

            return handler.Handle(@event);
        }
    }
}
