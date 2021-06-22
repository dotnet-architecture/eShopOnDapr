using System;
using System.Linq;
using System.Threading.Tasks;
using Dapr;
using Dapr.Actors;
using Dapr.Actors.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.eShopOnContainers.Services.Ordering.API.Actors;
using Microsoft.Extensions.Logging;
using Microsoft.eShopOnContainers.Services.Ordering.API.IntegrationEvents;

namespace Microsoft.eShopOnContainers.Services.Ordering.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class OrderingProcessEventController : ControllerBase
    {
        private const string DaprPubSubName = "pubsub";

        private readonly ILogger<OrderingProcessEventController> _logger;

        public OrderingProcessEventController(ILogger<OrderingProcessEventController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("UserCheckoutAccepted")]
        [Topic(DaprPubSubName, "UserCheckoutAcceptedIntegrationEvent")]
        public async Task HandleAsync(UserCheckoutAcceptedIntegrationEvent integrationEvent)
        {
            if (integrationEvent.RequestId != Guid.Empty)
            {
                var orderingProcess = GetOrderingProcessActor(integrationEvent.RequestId);

                await orderingProcess.SubmitAsync(
                    integrationEvent.UserId, integrationEvent.UserName,
                    integrationEvent.Street, integrationEvent.City, integrationEvent.ZipCode,
                    integrationEvent.State, integrationEvent.Country, integrationEvent.Basket);
            }
            else
            {
                _logger.LogWarning("Invalid IntegrationEvent - RequestId is missing - {@IntegrationEvent}", integrationEvent);
            }
        }

        [HttpPost("OrderStockConfirmed")]
        [Topic(DaprPubSubName, "OrderStockConfirmedIntegrationEvent")]
        public Task HandleAsync(OrderStockConfirmedIntegrationEvent integrationEvent)
        {
            return GetOrderingProcessActor(integrationEvent.OrderId)
                .NotifyStockConfirmedAsync();
        }

        [HttpPost("OrderStockRejected")]
        [Topic(DaprPubSubName, "OrderStockRejectedIntegrationEvent")]
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
        [Topic(DaprPubSubName, "OrderPaymentSucceededIntegrationEvent")]
        public Task HandleAsync(OrderPaymentSucceededIntegrationEvent integrationEvent)
        {
            return GetOrderingProcessActor(integrationEvent.OrderId)
                .NotifyPaymentSucceededAsync();
        }

        [HttpPost("OrderPaymentFailed")]
        [Topic(DaprPubSubName, "OrderPaymentFailedIntegrationEvent")]
        public Task HandleAsync(OrderPaymentFailedIntegrationEvent integrationEvent)
        {
            return GetOrderingProcessActor(integrationEvent.OrderId)
                .NotifyPaymentFailedAsync();
        }

        private static IOrderingProcessActor GetOrderingProcessActor(Guid orderId)
        {
            var actorId = new ActorId(orderId.ToString());
            return ActorProxy.Create<IOrderingProcessActor>(actorId, nameof(OrderingProcessActor));
        }
    }
}