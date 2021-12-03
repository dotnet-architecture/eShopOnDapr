namespace Microsoft.eShopOnDapr.Services.Payment.API.IntegrationEvents.EventHandling;

public class OrderStatusChangedToValidatedIntegrationEventHandler :
    IIntegrationEventHandler<OrderStatusChangedToValidatedIntegrationEvent>
{
    private readonly PaymentSettings _settings;
    private readonly IEventBus _eventBus;
    private readonly ILogger _logger;

    public OrderStatusChangedToValidatedIntegrationEventHandler(
        IOptions<PaymentSettings> settings,
        IEventBus eventBus,
        ILogger<OrderStatusChangedToValidatedIntegrationEventHandler> logger)
    {
        _settings = settings.Value;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task Handle(OrderStatusChangedToValidatedIntegrationEvent @event)
    {
        IntegrationEvent orderPaymentIntegrationEvent;

        // Business feature comment:
        // When OrderStatusChangedToValidated Integration Event is handled.
        // Here we're simulating that we'd be performing the payment against any payment gateway.
        // Instead of a real payment we just take the MaxOrderTotal to simulate payment approval.
        // The payment can be successful or it can fail

        await Task.Delay(3000); // Checking with the bank 😉

        if (_settings.PaymentSucceeded &&
            (!_settings.MaxOrderTotal.HasValue || @event.Total < _settings.MaxOrderTotal ))
        {
            orderPaymentIntegrationEvent = new OrderPaymentSucceededIntegrationEvent(@event.OrderId);
        }
        else
        {
            _logger.LogWarning(
                "Payment for ${Total} rejected for order {OrderId} because of service configuration",
                @event.Total,
                @event.OrderId);

            orderPaymentIntegrationEvent = new OrderPaymentFailedIntegrationEvent(@event.OrderId);
        }

        await _eventBus.PublishAsync(orderPaymentIntegrationEvent);
    }
}
