namespace Payment.API.IntegrationEvents.EventHandling
{
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Payment.API.IntegrationEvents.Events;
    using Serilog.Context;
    using System.Threading.Tasks;

    public class OrderStatusChangedToValidatedIntegrationEventHandler :
        IIntegrationEventHandler<OrderStatusChangedToValidatedIntegrationEvent>
    {
        private readonly IEventBus _eventBus;
        private readonly PaymentSettings _settings;
        private readonly ILogger<OrderStatusChangedToValidatedIntegrationEventHandler> _logger;

        public OrderStatusChangedToValidatedIntegrationEventHandler(
            IEventBus eventBus,
            IOptionsSnapshot<PaymentSettings> settings,
            ILogger<OrderStatusChangedToValidatedIntegrationEventHandler> logger)
        {
            _eventBus = eventBus;
            _settings = settings.Value;
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));

            _logger.LogTrace("PaymentSettings: {@PaymentSettings}", _settings);
        }

        public async Task Handle(OrderStatusChangedToValidatedIntegrationEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{Program.AppName}"))
            {
                _logger.LogInformation("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);

                IntegrationEvent orderPaymentIntegrationEvent;

                //Business feature comment:
                // When OrderStatusChangedToValidated Integration Event is handled.
                // Here we're simulating that we'd be performing the payment against any payment gateway
                // Instead of a real payment we just take the PaymentLimitToSucceed to simulate payment approval
                // The payment can be successful or it can fail

                await Task.Delay(3000); // Checking with the bank 😉

                if (_settings.PaymentSucceeded && (!_settings.MaxOrderTotal.HasValue || @event.Total < _settings.MaxOrderTotal ))
                {
                    orderPaymentIntegrationEvent = new OrderPaymentSucceededIntegrationEvent(@event.OrderId);
                }
                else
                {
                    _logger.LogWarning("----- Payment for ${Total} rejected for order {OrderId} because of service configuration", @event.Total, @event.OrderId);

                    orderPaymentIntegrationEvent = new OrderPaymentFailedIntegrationEvent(@event.OrderId);
                }

                _logger.LogInformation("----- Publishing integration event: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})", orderPaymentIntegrationEvent.Id, Program.AppName, orderPaymentIntegrationEvent);

                _eventBus.Publish(orderPaymentIntegrationEvent);

                await Task.CompletedTask;
            }
        }
    }
}