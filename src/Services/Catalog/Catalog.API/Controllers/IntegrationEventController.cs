namespace Microsoft.eShopOnDapr.Services.Catalog.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class IntegrationEventController : ControllerBase
{
    private const string DAPR_PUBSUB_NAME = "pubsub";

    [HttpPost("OrderStatusChangedToAwaitingStockValidation")]
    [Topic(DAPR_PUBSUB_NAME, nameof(OrderStatusChangedToAwaitingStockValidationIntegrationEvent))]
    public Task HandleAsync(
        OrderStatusChangedToAwaitingStockValidationIntegrationEvent @event,
        [FromServices] OrderStatusChangedToAwaitingStockValidationIntegrationEventHandler handler) =>
        handler.HandleAsync(@event);

    [HttpPost("OrderStatusChangedToPaid")]
    [Topic(DAPR_PUBSUB_NAME, "OrderStatusChangedToPaidIntegrationEvent")]
    public Task HandleAsync(
        OrderStatusChangedToPaidIntegrationEvent @event,
        [FromServices] OrderStatusChangedToPaidIntegrationEventHandler handler) =>
        handler.HandleAsync(@event);

    [HttpPost("VerifyProduct")]
    [Topic(DAPR_PUBSUB_NAME, "VerifyProductIntegrationEvent")]
    public Task HandleAsync(
        VerifyProductIntegrationEvent @event,
        [FromServices] VerifyProductIntegrationEventHandler handler) =>
        handler.HandleAsync(@event);
}
