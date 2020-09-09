using System;
using System.Threading.Tasks;
using Dapr;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Ordering.SignalrHub.IntegrationEvents;
using Ordering.SignalrHub.IntegrationEvents.EventHandling;
using Ordering.SignalrHub.IntegrationEvents.Events;

namespace Ordering.SignalrHub.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class IntegrationEventController : ControllerBase
    {
        private const string DAPR_PUBSUB_NAME = "pubsub";

        private readonly IServiceProvider _serviceProvider;

        public IntegrationEventController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        [HttpPost("OrderStatusChangedToAwaitingStockValidation")]
        [Topic(DAPR_PUBSUB_NAME, "OrderStatusChangedToAwaitingStockValidationIntegrationEvent")]
        public async Task OrderStarted(OrderStatusChangedToAwaitingStockValidationIntegrationEvent @event)
        {
            var handler = _serviceProvider.GetRequiredService<OrderStatusChangedToAwaitingStockValidationIntegrationEventHandler>();
            await handler.Handle(@event);
        }

        [HttpPost("OrderStatusChangedToCancelled")]
        [Topic(DAPR_PUBSUB_NAME, "OrderStatusChangedToCancelledIntegrationEvent")]
        public async Task OrderStarted(OrderStatusChangedToCancelledIntegrationEvent @event)
        {
            var handler = _serviceProvider.GetRequiredService<OrderStatusChangedToCancelledIntegrationEventHandler>();
            await handler.Handle(@event);
        }

        [HttpPost("OrderStatusChangedToPaid")]
        [Topic(DAPR_PUBSUB_NAME, "OrderStatusChangedToPaidIntegrationEvent")]
        public async Task OrderStarted(OrderStatusChangedToPaidIntegrationEvent @event)
        {
            var handler = _serviceProvider.GetRequiredService<OrderStatusChangedToPaidIntegrationEventHandler>();
            await handler.Handle(@event);
        }

        [HttpPost("OrderStatusChangedToShipped")]
        [Topic(DAPR_PUBSUB_NAME, "OrderStatusChangedToShippedIntegrationEvent")]
        public async Task OrderStarted(OrderStatusChangedToShippedIntegrationEvent @event)
        {
            var handler = _serviceProvider.GetRequiredService<OrderStatusChangedToShippedIntegrationEventHandler>();
            await handler.Handle(@event);
        }

        [HttpPost("OrderStatusChangedToSubmitted")]
        [Topic(DAPR_PUBSUB_NAME, "OrderStatusChangedToSubmittedIntegrationEvent")]
        public async Task OrderStarted(OrderStatusChangedToSubmittedIntegrationEvent @event)
        {
            var handler = _serviceProvider.GetRequiredService<OrderStatusChangedToSubmittedIntegrationEventHandler>();
            await handler.Handle(@event);
        }

        [HttpPost("OrderStatusChangedToValidated")]
        [Topic(DAPR_PUBSUB_NAME, "OrderStatusChangedToValidatedIntegrationEvent")]
        public async Task OrderStarted(OrderStatusChangedToValidatedIntegrationEvent @event)
        {
            var handler = _serviceProvider.GetRequiredService<OrderStatusChangedToValidatedIntegrationEventHandler>();
            await handler.Handle(@event);
        }
    }
}