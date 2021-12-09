namespace Microsoft.eShopOnDapr.Services.Basket.API.Controllers;

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
        => handler.HandleAsync(@event);

    [HttpPost("VerifiedBasketProduct")]
    [Topic(DAPR_PUBSUB_NAME, nameof(VerifiedBasketProductIntegrationEvent))]
    public Task HandleAsync(VerifiedBasketProductIntegrationEvent @event,
        [FromServices] VerifiedProductIntegrationEventHandler handler)
        => handler.HandleAsync(@event);
}
