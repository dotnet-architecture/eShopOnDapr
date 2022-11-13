namespace Microsoft.eShopOnDapr.Services.Ordering.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class OrderingProcessEventController : ControllerBase
{
    private const string DAPR_PUBSUB_NAME = "eshopondapr-pubsub";

    private readonly IActorProxyFactory _actorProxyFactory;
    private readonly ILogger<OrderingProcessEventController> _logger;

    public OrderingProcessEventController(
        IActorProxyFactory actorProxyFactory,
        ILogger<OrderingProcessEventController> logger)
    {
        _actorProxyFactory = actorProxyFactory;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpPost("UserCheckoutAccepted")]
    [Topic(DAPR_PUBSUB_NAME, "UserCheckoutAcceptedIntegrationEvent")]
    public async Task HandleAsync(UserCheckoutAcceptedIntegrationEvent integrationEvent)
    {
        if (integrationEvent.RequestId != Guid.Empty)
        {
            var orderingProcess = GetOrderingProcessActor(integrationEvent.RequestId);

            await orderingProcess.SubmitAsync(
                integrationEvent.UserId, integrationEvent.UserEmail, integrationEvent.Street, integrationEvent.City,
                integrationEvent.State, integrationEvent.Country, integrationEvent.Basket);
        }
        else
        {
            _logger.LogWarning("Invalid IntegrationEvent - RequestId is missing - {@IntegrationEvent}", integrationEvent);
        }
    }

    [HttpPost("OrderStockConfirmed")]
    [Topic(DAPR_PUBSUB_NAME, "OrderStockConfirmedIntegrationEvent")]
    public Task HandleAsync(OrderStockConfirmedIntegrationEvent integrationEvent)
    {
        return GetOrderingProcessActor(integrationEvent.OrderId)
            .NotifyStockConfirmedAsync();
    }

    [HttpPost("OrderStockRejected")]
    [Topic(DAPR_PUBSUB_NAME, "OrderStockRejectedIntegrationEvent")]
    public Task HandleAsync(OrderStockRejectedIntegrationEvent integrationEvent)
    {
        var outOfStockItems = integrationEvent.OrderStockItems
            .FindAll(c => !c.HasStock)
            .Select(c => c.ProductId)
            .ToList();

        return GetOrderingProcessActor(integrationEvent.OrderId)
            .NotifyStockRejectedAsync(outOfStockItems);
    }

    [HttpPost("OrderPaymentSucceeded")]
    [Topic(DAPR_PUBSUB_NAME, "OrderPaymentSucceededIntegrationEvent")]
    public Task HandleAsync(OrderPaymentSucceededIntegrationEvent integrationEvent)
    {
        return GetOrderingProcessActor(integrationEvent.OrderId)
            .NotifyPaymentSucceededAsync();
    }

    [HttpPost("OrderPaymentFailed")]
    [Topic(DAPR_PUBSUB_NAME, "OrderPaymentFailedIntegrationEvent")]
    public Task HandleAsync(OrderPaymentFailedIntegrationEvent integrationEvent)
    {
        return GetOrderingProcessActor(integrationEvent.OrderId)
            .NotifyPaymentFailedAsync();
    }

    private IOrderingProcessActor GetOrderingProcessActor(Guid orderId)
    {
        var actorId = new ActorId(orderId.ToString());
        return _actorProxyFactory.CreateActorProxy<IOrderingProcessActor>(
            actorId,
            nameof(OrderingProcessActor));
    }
}
