using System.Threading.Tasks;
using Dapr;
using Microsoft.AspNetCore.Mvc;
using Microsoft.eShopOnDapr.Services.Basket.API.IntegrationEvents.EventHandling;
using Microsoft.eShopOnDapr.Services.Basket.API.IntegrationEvents.Events;

namespace Microsoft.eShopOnDapr.Services.Basket.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class IntegrationEventController : ControllerBase
    {
        private const string DAPR_PUBSUB_NAME = "pubsub";

        [HttpPost("OrderStatusChangedToSubmitted")]
        [Topic(DAPR_PUBSUB_NAME, nameof(OrderStatusChangedToSubmittedIntegrationEvent))]
        public Task HandleAsync(
            OrderStatusChangedToSubmittedIntegrationEvent @event,
            [FromServices] OrderStatusChangedToSubmittedIntegrationEventHandler handler)
            => handler.Handle(@event);
    }
}
