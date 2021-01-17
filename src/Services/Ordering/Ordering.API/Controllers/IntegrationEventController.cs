using System;
using System.Linq;
using System.Threading.Tasks;
using Dapr;
using Dapr.Actors;
using Dapr.Actors.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.eShopOnContainers.Services.Ordering.API.Actors;
using Microsoft.eShopOnContainers.Services.Ordering.API.Model;
using Microsoft.Extensions.Logging;
using Ordering.API.Application.IntegrationEvents.Events;

namespace Microsoft.eShopOnContainers.Services.Ordering.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class IntegrationEventController : ControllerBase
    {
        private const string DaprPubSubName = "pubsub";

        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<IntegrationEventController> _logger;

        public IntegrationEventController(IOrderRepository orderRepository, ILogger<IntegrationEventController> logger)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("UserCheckoutAccepted")]
        [Topic(DaprPubSubName, "UserCheckoutAcceptedIntegrationEvent")]
        public async Task Handle(UserCheckoutAcceptedIntegrationEvent integrationEvent)
        {
            if (integrationEvent.RequestId != Guid.Empty)
            {
                var order = new Order
                {
                    RequestId = integrationEvent.RequestId,
                    OrderDate = DateTime.UtcNow,
                    Address = new Address
                    {
                        Street = integrationEvent.Street,
                        City = integrationEvent.City,
                        ZipCode = integrationEvent.ZipCode,
                        State = integrationEvent.State,
                        Country = integrationEvent.Country
                    },
                    OrderStatus = OrderStatus.Submitted,
                    BuyerId = integrationEvent.UserId,
                    BuyerName = integrationEvent.UserName,
                    PaymentMethodId = integrationEvent.CardTypeId,
                    OrderItems = integrationEvent.Basket.Items
                        .Select(item => new OrderItem
                        {
                            ProductId = item.ProductId,
                            ProductName = item.ProductName,
                            UnitPrice = item.UnitPrice,
                            Units = item.Quantity,
                            PictureUrl = item.PictureUrl
                        })
                        .ToList()
                };

                // TODO Why not set BuyerId from integrationEvent.Basket?
                // Event
                //AddOrderStartedDomainEvent(userId, userName, cardTypeId, cardNumber,
                //            cardSecurityNumber, cardHolderName, cardExpiration);

                _logger.LogInformation("----- Creating Order - Order: {@Order}", order);

                order = await _orderRepository.GetOrAddOrderAsync(order);

                var orderingProcess = GetOrderingProcessActor(order.Id);
                await orderingProcess.Start(-1, order.BuyerName, order.PaymentMethodId);
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
            var orderingProcess = GetOrderingProcessActor(integrationEvent.OrderId);

            return orderingProcess.NotifyStockConfirmed();
        }

        [HttpPost("OrderStockRejected")]
        [Topic(DaprPubSubName, "OrderStockRejectedIntegrationEvent")]
        public Task Handle(OrderStockRejectedIntegrationEvent integrationEvent)
        {
            var orderingProcess = GetOrderingProcessActor(integrationEvent.OrderId);

            var orderStockRejectedItems = integrationEvent.OrderStockItems
                    .FindAll(c => !c.HasStock)
                    .Select(c => c.ProductId)
                    .ToList();

            return orderingProcess.NotifyStockRejected(orderStockRejectedItems);
        }

        [HttpPost("OrderPaymentSucceeded")]
        [Topic(DaprPubSubName, "OrderPaymentSucceededIntegrationEvent")]
        public Task Handle(OrderPaymentSucceededIntegrationEvent integrationEvent)
        {
            var orderingProcess = GetOrderingProcessActor(integrationEvent.OrderId);

            return orderingProcess.NotifyPaymentSucceeded();
        }

        [HttpPost("OrderPaymentFailed")]
        [Topic(DaprPubSubName, "OrderPaymentFailedIntegrationEvent")]
        public Task Handle(OrderPaymentFailedIntegrationEvent integrationEvent)
        {
            var orderingProcess = GetOrderingProcessActor(integrationEvent.OrderId);

            return orderingProcess.NotifyPaymentFailed();
        }

        private static IOrderingProcessActor GetOrderingProcessActor(int orderId)
        {
            var actorId = new ActorId(orderId.ToString());
            return ActorProxy.Create<IOrderingProcessActor>(actorId, nameof(OrderingProcessActor));
        }
    }
}