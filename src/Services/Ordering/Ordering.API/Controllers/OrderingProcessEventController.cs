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
        public async Task Handle(UserCheckoutAcceptedIntegrationEvent integrationEvent)
        {
            if (integrationEvent.RequestId != Guid.Empty)
            {
                var orderingProcess = GetOrderingProcessActor(integrationEvent.RequestId);

                await orderingProcess.Submit(integrationEvent.UserId, integrationEvent.UserName,
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
        public Task Handle(OrderStockConfirmedIntegrationEvent integrationEvent)
        {
            return GetOrderingProcessActor(integrationEvent.OrderId).NotifyStockConfirmed();
        }

        [HttpPost("OrderStockRejected")]
        [Topic(DaprPubSubName, "OrderStockRejectedIntegrationEvent")]
        public Task Handle(OrderStockRejectedIntegrationEvent integrationEvent)
        {
            return GetOrderingProcessActor(integrationEvent.OrderId).NotifyStockRejected(
                integrationEvent.OrderStockItems
                    .FindAll(c => !c.HasStock)
                    .Select(c => c.ProductId)
                    .ToList());
        }

        [HttpPost("OrderPaymentSucceeded")]
        [Topic(DaprPubSubName, "OrderPaymentSucceededIntegrationEvent")]
        public Task Handle(OrderPaymentSucceededIntegrationEvent integrationEvent)
        {
            return GetOrderingProcessActor(integrationEvent.OrderId).NotifyPaymentSucceeded();
        }

        [HttpPost("OrderPaymentFailed")]
        [Topic(DaprPubSubName, "OrderPaymentFailedIntegrationEvent")]
        public Task Handle(OrderPaymentFailedIntegrationEvent integrationEvent)
        {
            return GetOrderingProcessActor(integrationEvent.OrderId).NotifyPaymentFailed();
        }

        private static IOrderingProcessActor GetOrderingProcessActor(Guid orderId)
        {
            var actorId = new ActorId(orderId.ToString());
            return ActorProxy.Create<IOrderingProcessActor>(actorId, nameof(OrderingProcessActor));
        }
    }
}