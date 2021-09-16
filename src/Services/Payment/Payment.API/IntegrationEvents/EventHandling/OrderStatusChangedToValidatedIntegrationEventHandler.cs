using System.Threading.Tasks;
using Microsoft.eShopOnDapr.BuildingBlocks.EventBus.Abstractions;
using Microsoft.eShopOnDapr.BuildingBlocks.EventBus.Events;
using Microsoft.eShopOnDapr.Services.Payment.API.IntegrationEvents.Events;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Microsoft.eShopOnDapr.Services.Payment.API.IntegrationEvents.EventHandling
{
    public class OrderStatusChangedToValidatedIntegrationEventHandler :
        IIntegrationEventHandler<OrderStatusChangedToValidatedIntegrationEvent>
    {
        private readonly PaymentSettings _settings;
        private readonly IEventBus _eventBus;
        private readonly ILogger _logger;

        public OrderStatusChangedToValidatedIntegrationEventHandler(
            IOptionsSnapshot<PaymentSettings> settings,
            IEventBus eventBus,
            ILogger<OrderStatusChangedToValidatedIntegrationEventHandler> logger)
        {
            if (settings == null)
            {
                throw new System.ArgumentNullException(nameof(logger));
            }

            _settings = settings.Value;
            _eventBus = eventBus ?? throw new System.ArgumentNullException(nameof(logger));
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
        }

        public async Task Handle(OrderStatusChangedToValidatedIntegrationEvent @event)
        {
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
                _logger.LogWarning(
                    "Payment for ${Total} rejected for order {OrderId} because of service configuration",
                    @event.Total,
                    @event.OrderId);

                orderPaymentIntegrationEvent = new OrderPaymentFailedIntegrationEvent(@event.OrderId);
            }

            await _eventBus.PublishAsync(orderPaymentIntegrationEvent);
        }
    }
}