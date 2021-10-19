using System.Threading.Tasks;
using Dapr;
using Microsoft.AspNetCore.Mvc;
using Microsoft.eShopOnDapr.Services.Payment.API.IntegrationEvents.EventHandling;
using Microsoft.eShopOnDapr.Services.Payment.API.IntegrationEvents.Events;

namespace Microsoft.eShopOnDapr.Services.Payment.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class IntegrationEventController : ControllerBase
    {
        private const string DAPR_PUBSUB_NAME = "pubsub";

        [HttpPost("OrderStatusChangedToValidated")]
        [Topic(DAPR_PUBSUB_NAME, nameof(OrderStatusChangedToValidatedIntegrationEvent))]
        public Task HandleAsync(
            OrderStatusChangedToValidatedIntegrationEvent @event,
            [FromServices] OrderStatusChangedToValidatedIntegrationEventHandler handler)
            => handler.Handle(@event);
    }
}