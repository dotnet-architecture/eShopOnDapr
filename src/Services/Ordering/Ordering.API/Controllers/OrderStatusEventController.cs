using System;
using System.Threading.Tasks;
using Dapr;
using Dapr.Actors;
using Dapr.Actors.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.eShopOnContainers.Services.Ordering.API.Actors;
using Microsoft.eShopOnContainers.Services.Ordering.API.Model;
using Microsoft.Extensions.Logging;
using Ordering.API.Application.IntegrationEvents.Events;

namespace Microsoft.eShopOnContainers.Services.Ordering.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class OrderStatusEventController : ControllerBase
    {
        private const string DaprPubSubName = "pubsub";

        private readonly IOrderRepository _orderRepository;
        private readonly IHubContext<NotificationsHub> _hubContext;
        private readonly ILogger<OrderStatusEventController> _logger;

        public OrderStatusEventController(IOrderRepository orderRepository,
            IHubContext<NotificationsHub> hubContext, ILogger<OrderStatusEventController> logger)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("OrderStatusChangedToSubmitted")]
        [Topic(DaprPubSubName, "OrderStatusChangedToSubmittedIntegrationEvent")]
        public async Task Handle(OrderStatusChangedToSubmittedIntegrationEvent integrationEvent)
        {
            var actorId = new ActorId(integrationEvent.OrderId.ToString());
            var orderingProcess = ActorProxy.Create<IOrderingProcessActor>(actorId, nameof(OrderingProcessActor));

            var actorOrder = await orderingProcess.GetOrderDetails();
            var readModelOrder = Model.Order.FromActorState(integrationEvent.OrderId, actorOrder);

            readModelOrder = await _orderRepository.AddOrGetOrderAsync(readModelOrder);

            await SendNotificationAsync(readModelOrder.OrderNumber, integrationEvent.OrderStatus,
                integrationEvent.BuyerName);
        }

        [HttpPost("OrderStatusChangedToAwaitingStockValidation")]
        [Topic(DaprPubSubName, "OrderStatusChangedToAwaitingStockValidationIntegrationEvent")]
        public Task Handle(OrderStatusChangedToAwaitingStockValidationIntegrationEvent integrationEvent)
        {
            return UpdateReadModelAndSendNotificationAsync(integrationEvent.OrderId,
                integrationEvent.OrderStatus, integrationEvent.Description, integrationEvent.BuyerName);
        }

        [HttpPost("OrderStatusChangedToValidated")]
        [Topic(DaprPubSubName, "OrderStatusChangedToValidatedIntegrationEvent")]
        public Task Handle(OrderStatusChangedToValidatedIntegrationEvent integrationEvent)
        {
            return UpdateReadModelAndSendNotificationAsync(integrationEvent.OrderId,
                integrationEvent.OrderStatus, integrationEvent.Description, integrationEvent.BuyerName);
        }

        [HttpPost("OrderStatusChangedToPaid")]
        [Topic(DaprPubSubName, "OrderStatusChangedToPaidIntegrationEvent")]
        public Task Handle(OrderStatusChangedToPaidIntegrationEvent integrationEvent)
        {
            return UpdateReadModelAndSendNotificationAsync(integrationEvent.OrderId,
                integrationEvent.OrderStatus, integrationEvent.Description, integrationEvent.BuyerName);
        }

        [HttpPost("OrderStatusChangedToShipped")]
        [Topic(DaprPubSubName, "OrderStatusChangedToShippedIntegrationEvent")]
        public Task Handle(OrderStatusChangedToShippedIntegrationEvent integrationEvent)
        {
            return UpdateReadModelAndSendNotificationAsync(integrationEvent.OrderId,
                integrationEvent.OrderStatus, integrationEvent.Description, integrationEvent.BuyerName);
        }

        [HttpPost("OrderStatusChangedToCancelled")]
        [Topic(DaprPubSubName, "OrderStatusChangedToCancelledIntegrationEvent")]
        public Task Handle(OrderStatusChangedToCancelledIntegrationEvent integrationEvent)
        {
            return UpdateReadModelAndSendNotificationAsync(integrationEvent.OrderId,
                integrationEvent.OrderStatus, integrationEvent.Description, integrationEvent.BuyerName);
        }

        private async Task UpdateReadModelAndSendNotificationAsync(Guid orderId, string orderStatus,
            string description, string buyerName)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            order.OrderStatus = orderStatus;
            order.Description = description;

            await _orderRepository.UpdateOrderAsync(order);
            await SendNotificationAsync(order.OrderNumber, orderStatus, buyerName);
        }

        private Task SendNotificationAsync(int orderNumber, string orderStatus, string buyerName)
        {
            return _hubContext.Clients
                .Group(buyerName)
                .SendAsync("UpdatedOrderState", new { OrderNumber = orderNumber, Status = orderStatus });
        }
    }
}