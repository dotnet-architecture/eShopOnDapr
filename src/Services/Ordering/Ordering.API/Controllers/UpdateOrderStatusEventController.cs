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
using Microsoft.Extensions.Options;
using Microsoft.eShopOnContainers.Services.Ordering.API.IntegrationEvents;

namespace Microsoft.eShopOnContainers.Services.Ordering.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UpdateOrderStatusEventController : ControllerBase
    {
        private const string DaprPubSubName = "pubsub";

        private readonly IOrderRepository _orderRepository;
        private readonly IHubContext<NotificationsHub> _hubContext;
        private readonly ILogger<UpdateOrderStatusEventController> _logger;

        public UpdateOrderStatusEventController(IOrderRepository orderRepository,
            IHubContext<NotificationsHub> hubContext, ILogger<UpdateOrderStatusEventController> logger)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("OrderStatusChangedToSubmitted")]
        [Topic(DaprPubSubName, "OrderStatusChangedToSubmittedIntegrationEvent")]
        public async Task Handle(OrderStatusChangedToSubmittedIntegrationEvent integrationEvent,
            [FromServices] IOptions<OrderingSettings> settings, [FromServices] IEmailService emailService)
        {
            // Gets the order details from Actor state.
            var actorId = new ActorId(integrationEvent.OrderId.ToString());
            var orderingProcess = ActorProxy.Create<IOrderingProcessActor>(actorId, nameof(OrderingProcessActor));
            //
            var actorOrder = await orderingProcess.GetOrderDetails();
            var readModelOrder = Model.Order.FromActorState(integrationEvent.OrderId, actorOrder);

            // Add the order to the read model so it can be queried from the API.
            // It may already exist if this event has been handled before (at-least-once semantics).
            readModelOrder = await _orderRepository.AddOrGetOrderAsync(readModelOrder);

            // Send a SignalR notification to the client.
            await SendNotificationAsync(readModelOrder.OrderNumber, integrationEvent.OrderStatus,
                integrationEvent.BuyerName);

            // Send a confirmation e-mail if enabled.
            if (settings.Value.SendConfirmationEmail)
            {
                await emailService.SendOrderConfirmation(readModelOrder);
            }
        }

        [HttpPost("OrderStatusChangedToAwaitingStockValidation")]
        [Topic(DaprPubSubName, "OrderStatusChangedToAwaitingStockValidationIntegrationEvent")]
        public Task Handle(OrderStatusChangedToAwaitingStockValidationIntegrationEvent integrationEvent)
        {
            // Save the updated status in the read model and notify the client via SignalR.
            return UpdateReadModelAndSendNotificationAsync(integrationEvent.OrderId,
                integrationEvent.OrderStatus, integrationEvent.Description, integrationEvent.BuyerName);
        }

        [HttpPost("OrderStatusChangedToValidated")]
        [Topic(DaprPubSubName, "OrderStatusChangedToValidatedIntegrationEvent")]
        public Task Handle(OrderStatusChangedToValidatedIntegrationEvent integrationEvent)
        {
            // Save the updated status in the read model and notify the client via SignalR.
            return UpdateReadModelAndSendNotificationAsync(integrationEvent.OrderId,
                integrationEvent.OrderStatus, integrationEvent.Description, integrationEvent.BuyerName);
        }

        [HttpPost("OrderStatusChangedToPaid")]
        [Topic(DaprPubSubName, "OrderStatusChangedToPaidIntegrationEvent")]
        public Task Handle(OrderStatusChangedToPaidIntegrationEvent integrationEvent)
        {
            // Save the updated status in the read model and notify the client via SignalR.
            return UpdateReadModelAndSendNotificationAsync(integrationEvent.OrderId,
                integrationEvent.OrderStatus, integrationEvent.Description, integrationEvent.BuyerName);
        }

        [HttpPost("OrderStatusChangedToShipped")]
        [Topic(DaprPubSubName, "OrderStatusChangedToShippedIntegrationEvent")]
        public Task Handle(OrderStatusChangedToShippedIntegrationEvent integrationEvent)
        {
            // Save the updated status in the read model and notify the client via SignalR.
            return UpdateReadModelAndSendNotificationAsync(integrationEvent.OrderId,
                integrationEvent.OrderStatus, integrationEvent.Description, integrationEvent.BuyerName);
        }

        [HttpPost("OrderStatusChangedToCancelled")]
        [Topic(DaprPubSubName, "OrderStatusChangedToCancelledIntegrationEvent")]
        public Task Handle(OrderStatusChangedToCancelledIntegrationEvent integrationEvent)
        {
            // Save the updated status in the read model and notify the client via SignalR.
            return UpdateReadModelAndSendNotificationAsync(integrationEvent.OrderId,
                integrationEvent.OrderStatus, integrationEvent.Description, integrationEvent.BuyerName);
        }

        private async Task UpdateReadModelAndSendNotificationAsync(Guid orderId, string orderStatus,
            string description, string buyerName)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            if (order != null)
            {
                order.OrderStatus = orderStatus;
                order.Description = description;

                await _orderRepository.UpdateOrderAsync(order);
                await SendNotificationAsync(order.OrderNumber, orderStatus, buyerName);
            }
        }

        private Task SendNotificationAsync(int orderNumber, string orderStatus, string buyerName)
        {
            return _hubContext.Clients
                .Group(buyerName)
                .SendAsync("UpdatedOrderState", new { OrderNumber = orderNumber, Status = orderStatus });
        }
    }
}