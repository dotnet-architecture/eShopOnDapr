using System.Threading.Tasks;
using Dapr;
using Microsoft.AspNetCore.Mvc;
using Microsoft.eShopOnDapr.Services.Catalog.API.IntegrationEvents.EventHandling;
using Microsoft.eShopOnDapr.Services.Catalog.API.IntegrationEvents.Events;

namespace Microsoft.eShopOnDapr.Services.Catalog.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class IntegrationEventController : ControllerBase
    {
        private const string DAPR_PUBSUB_NAME = "pubsub";

        [HttpPost("OrderStatusChangedToAwaitingStockValidation")]
        [Topic(DAPR_PUBSUB_NAME, nameof(OrderStatusChangedToAwaitingStockValidationIntegrationEvent))]
        public Task HandleAsync(
            OrderStatusChangedToAwaitingStockValidationIntegrationEvent @event,
            [FromServices] OrderStatusChangedToAwaitingStockValidationIntegrationEventHandler handler)
            => handler.Handle(@event);

        [HttpPost("OrderStatusChangedToPaid")]
        [Topic(DAPR_PUBSUB_NAME, "OrderStatusChangedToPaidIntegrationEvent")]
        public Task HandleAsync(
            OrderStatusChangedToPaidIntegrationEvent @event,
            [FromServices] OrderStatusChangedToPaidIntegrationEventHandler handler)
            => handler.Handle(@event);
    }
}