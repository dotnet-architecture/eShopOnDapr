using System;
using System.Threading.Tasks;
using Dapr;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Ordering.API.Application.IntegrationEvents.EventHandling;
using Ordering.API.Application.IntegrationEvents.Events;

namespace Microsoft.eShopOnContainers.Services.Ordering.API.Controllers
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

        [HttpPost("UserCheckoutAccepted")]
        [Topic(DAPR_PUBSUB_NAME, "UserCheckoutAcceptedIntegrationEvent")]
        public async Task OrderStarted(UserCheckoutAcceptedIntegrationEvent @event)
        {
            var handler = _serviceProvider.GetRequiredService<UserCheckoutAcceptedIntegrationEventHandler>();
            await handler.Handle(@event);
        }

        [HttpPost("GracePeriodConfirmedIntegration")]
        [Topic(DAPR_PUBSUB_NAME, "GracePeriodConfirmedIntegrationEvent")]
        public async Task OrderStarted(GracePeriodConfirmedIntegrationEvent @event)
        {
            var handler = _serviceProvider.GetRequiredService<GracePeriodConfirmedIntegrationEventHandler>();
            await handler.Handle(@event);
        }

        [HttpPost("OrderStockConfirmed")]
        [Topic(DAPR_PUBSUB_NAME, "OrderStockConfirmedIntegrationEvent")]
        public async Task OrderStarted(OrderStockConfirmedIntegrationEvent @event)
        {
            var handler = _serviceProvider.GetRequiredService<OrderStockConfirmedIntegrationEventHandler>();
            await handler.Handle(@event);
        }

        [HttpPost("OrderStockRejected")]
        [Topic(DAPR_PUBSUB_NAME, "OrderStockRejectedIntegrationEvent")]
        public async Task OrderStarted(OrderStockRejectedIntegrationEvent @event)
        {
            var handler = _serviceProvider.GetRequiredService<OrderStockRejectedIntegrationEventHandler>();
            await handler.Handle(@event);
        }

        [HttpPost("OrderPaymentFailed")]
        [Topic(DAPR_PUBSUB_NAME, "OrderPaymentFailedIntegrationEvent")]
        public async Task OrderStarted(OrderPaymentFailedIntegrationEvent @event)
        {
            var handler = _serviceProvider.GetRequiredService<OrderPaymentFailedIntegrationEventHandler>();
            await handler.Handle(@event);
        }

        [HttpPost("OrderPaymentSucceeded")]
        [Topic(DAPR_PUBSUB_NAME, "OrderPaymentSucceededIntegrationEvent")]
        public async Task OrderStarted(OrderPaymentSucceededIntegrationEvent @event)
        {
            var handler = _serviceProvider.GetRequiredService<OrderPaymentSucceededIntegrationEventHandler>();
            await handler.Handle(@event);
        }
    }
}