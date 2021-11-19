namespace Microsoft.eShopOnDapr.Services.Ordering.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class UpdateOrderStatusEventController : ControllerBase
{
    private const string DAPR_PUBSUB_NAME = "pubsub";

    private readonly IOrderRepository _orderRepository;
    private readonly IHubContext<NotificationsHub> _hubContext;
    private readonly IActorProxyFactory _actorProxyFactory;
    private readonly ILogger<UpdateOrderStatusEventController> _logger;

    public UpdateOrderStatusEventController(
        IOrderRepository orderRepository,
        IHubContext<NotificationsHub> hubContext,
        IActorProxyFactory actorProxyFactory,
        ILogger<UpdateOrderStatusEventController> logger)
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        _actorProxyFactory = actorProxyFactory ?? throw new ArgumentNullException(nameof(actorProxyFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpPost("OrderStatusChangedToSubmitted")]
    [Topic(DAPR_PUBSUB_NAME, nameof(OrderStatusChangedToSubmittedIntegrationEvent))]
    public async Task HandleAsync(
        OrderStatusChangedToSubmittedIntegrationEvent integrationEvent,
        [FromServices] IOptions<OrderingSettings> settings,
        [FromServices] IEmailService emailService)
    {
        // Gets the order details from Actor state.
        var actorId = new ActorId(integrationEvent.OrderId.ToString());
        var orderingProcess = _actorProxyFactory.CreateActorProxy<IOrderingProcessActor>(
            actorId,
            nameof(OrderingProcessActor));
        //
        var actorOrder = await orderingProcess.GetOrderDetails();
        var readModelOrder = new Order(integrationEvent.OrderId, actorOrder);

        // Add the order to the read model so it can be queried from the API.
        // It may already exist if this event has been handled before (at-least-once semantics).
        readModelOrder = await _orderRepository.AddOrGetOrderAsync(readModelOrder);

        // Send a SignalR notification to the client.
        await SendNotificationAsync(readModelOrder.OrderNumber, integrationEvent.OrderStatus, integrationEvent.BuyerId);

        // Send a confirmation e-mail if enabled.
        if (settings.Value.SendConfirmationEmail)
        {
            await emailService.SendOrderConfirmationAsync(readModelOrder);
        }
    }

    [HttpPost("OrderStatusChangedToAwaitingStockValidation")]
    [Topic(DAPR_PUBSUB_NAME, nameof(OrderStatusChangedToAwaitingStockValidationIntegrationEvent))]
    public Task HandleAsync(
        OrderStatusChangedToAwaitingStockValidationIntegrationEvent integrationEvent)
    {
        // Save the updated status in the read model and notify the client via SignalR.
        return UpdateReadModelAndSendNotificationAsync(integrationEvent.OrderId,
            integrationEvent.OrderStatus, integrationEvent.Description, integrationEvent.BuyerId);
    }

    [HttpPost("OrderStatusChangedToValidated")]
    [Topic(DAPR_PUBSUB_NAME, nameof(OrderStatusChangedToValidatedIntegrationEvent))]
    public Task HandleAsync(
        OrderStatusChangedToValidatedIntegrationEvent integrationEvent)
    {
        // Save the updated status in the read model and notify the client via SignalR.
        return UpdateReadModelAndSendNotificationAsync(integrationEvent.OrderId,
            integrationEvent.OrderStatus, integrationEvent.Description, integrationEvent.BuyerId);
    }

    [HttpPost("OrderStatusChangedToPaid")]
    [Topic(DAPR_PUBSUB_NAME, nameof(OrderStatusChangedToPaidIntegrationEvent))]
    public Task HandleAsync(
        OrderStatusChangedToPaidIntegrationEvent integrationEvent)
    {
        // Save the updated status in the read model and notify the client via SignalR.
        return UpdateReadModelAndSendNotificationAsync(integrationEvent.OrderId,
            integrationEvent.OrderStatus, integrationEvent.Description, integrationEvent.BuyerId);
    }

    [HttpPost("OrderStatusChangedToShipped")]
    [Topic(DAPR_PUBSUB_NAME, nameof(OrderStatusChangedToShippedIntegrationEvent))]
    public Task HandleAsync(
        OrderStatusChangedToShippedIntegrationEvent integrationEvent)
    {
        // Save the updated status in the read model and notify the client via SignalR.
        return UpdateReadModelAndSendNotificationAsync(integrationEvent.OrderId,
            integrationEvent.OrderStatus, integrationEvent.Description, integrationEvent.BuyerId);
    }

    [HttpPost("OrderStatusChangedToCancelled")]
    [Topic(DAPR_PUBSUB_NAME, nameof(OrderStatusChangedToCancelledIntegrationEvent))]
    public Task HandleAsync(
        OrderStatusChangedToCancelledIntegrationEvent integrationEvent)
    {
        // Save the updated status in the read model and notify the client via SignalR.
        return UpdateReadModelAndSendNotificationAsync(integrationEvent.OrderId,
            integrationEvent.OrderStatus, integrationEvent.Description, integrationEvent.BuyerId);
    }

    private async Task UpdateReadModelAndSendNotificationAsync(
        Guid orderId, string orderStatus, string description, string buyerId)
    {
        var order = await _orderRepository.GetOrderByIdAsync(orderId);
        if (order is not null)
        {
            order.OrderStatus = orderStatus;
            order.Description = description;

            await _orderRepository.UpdateOrderAsync(order);
            await SendNotificationAsync(order.OrderNumber, orderStatus, buyerId);
        }
    }

    private Task SendNotificationAsync(
        int orderNumber, string orderStatus, string buyerId)
    {
        return _hubContext.Clients
            .Group(buyerId)
            .SendAsync("UpdatedOrderState", new { OrderNumber = orderNumber, Status = orderStatus });
    }
}
