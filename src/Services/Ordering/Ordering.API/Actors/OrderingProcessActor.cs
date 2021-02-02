using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapr.Actors.Runtime;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using Microsoft.eShopOnContainers.Services.Ordering.API.IntegrationEvents;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Microsoft.eShopOnContainers.Services.Ordering.API.Application.Models;

namespace Microsoft.eShopOnContainers.Services.Ordering.API.Actors
{
    public class OrderingProcessActor : Actor, IOrderingProcessActor, IRemindable
    {
        private const string OrderDetailsStateName = "OrderDetails";
        private const string OrderStatusStateName = "OrderStatus";

        private const string GracePeriodElapsedReminder = "GracePeriodElapsed";
        private const string StockConfirmedReminder = "StockConfirmed";
        private const string StockRejectedReminder = "StockRejected";
        private const string PaymentSucceededReminder = "PaymentSucceeded";
        private const string PaymentFailedReminder = "PaymentFailed";

        private readonly IEventBus _eventBus;
        private readonly IOptions<OrderingSettings> _settings;
        private readonly ILogger<OrderingProcessActor> _logger;

        private int? _preMethodOrderStatusId;

        public OrderingProcessActor(ActorHost host, IEventBus eventBus, IOptions<OrderingSettings> settings,
            ILogger<OrderingProcessActor> logger)
            : base(host)
        {
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private Guid OrderId => Guid.Parse(Id.GetId());

        public async Task Submit(string userId, string userName, string street, string city,
            string zipCode, string state, string country, CustomerBasket basket)
        {
            var order = new Order
            {
                OrderDate = DateTime.UtcNow,
                OrderStatus = OrderStatus.Submitted,
                UserId = userId,
                UserName = userName,
                Address = new OrderAddress
                {
                    Street = street,
                    City = city,
                    ZipCode = zipCode,
                    State = state,
                    Country = country
                },
                OrderItems = basket.Items
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

            await StateManager.SetStateAsync(OrderDetailsStateName, order);
            await StateManager.SetStateAsync(OrderStatusStateName, OrderStatus.Submitted);

            await RegisterReminderAsync(
                GracePeriodElapsedReminder,
                null,
                TimeSpan.FromSeconds(_settings.Value.GracePeriodTime),
                TimeSpan.FromMilliseconds(-1));

            await _eventBus.PublishAsync(new OrderStatusChangedToSubmittedIntegrationEvent(
                OrderId,
                OrderStatus.Submitted.Name,
                userId,
                userName));
        }

        public async Task NotifyStockConfirmed()
        {
            var statusChanged = await TryUpdateOrderStatusAsync(OrderStatus.AwaitingStockValidation, OrderStatus.Validated);
            if (statusChanged)
            {
                // Simulate a work time by setting a reminder.
                await RegisterReminderAsync(
                    StockConfirmedReminder,
                    null,
                    TimeSpan.FromSeconds(10),
                    TimeSpan.FromMilliseconds(-1));
            }            
        }

        public async Task NotifyStockRejected(List<int> rejectedProductIds)
        {
            var statusChanged = await TryUpdateOrderStatusAsync(OrderStatus.AwaitingStockValidation, OrderStatus.Cancelled);
            if (statusChanged)
            {
                var reminderState = JsonConvert.SerializeObject(rejectedProductIds);

                // Simulate a work time by setting a reminder.
                await RegisterReminderAsync(
                    StockRejectedReminder,
                    Encoding.UTF8.GetBytes(reminderState),
                    TimeSpan.FromSeconds(10),
                    TimeSpan.FromMilliseconds(-1));
            }
        }

        public async Task NotifyPaymentSucceeded()
        {
            var statusChanged = await TryUpdateOrderStatusAsync(OrderStatus.Validated, OrderStatus.Paid);
            if (statusChanged)
            {
                // Simulate a work time by setting a reminder.
                await RegisterReminderAsync(
                    PaymentSucceededReminder,
                    null,
                    TimeSpan.FromSeconds(10),
                    TimeSpan.FromMilliseconds(-1));
            }
        }

        public async Task NotifyPaymentFailed()
        {
            var statusChanged = await TryUpdateOrderStatusAsync(OrderStatus.Validated, OrderStatus.Paid);
            if (statusChanged)
            {
                // Simulate a work time by setting a reminder.
                await RegisterReminderAsync(
                    PaymentFailedReminder,
                    null,
                    TimeSpan.FromSeconds(10),
                    TimeSpan.FromMilliseconds(-1));
            }
        }

        public async Task<bool> Cancel()
        {
            var orderStatus = await StateManager.TryGetStateAsync<OrderStatus>(OrderStatusStateName);
            if (!orderStatus.HasValue)
            {
                _logger.LogWarning("Order with Id: {OrderId} cannot be cancelled because it doesn't exist",
                    OrderId);

                return false;
            }

            if ( orderStatus.Value.Id == OrderStatus.Paid.Id || orderStatus.Value.Id == OrderStatus.Shipped.Id)
            {
                _logger.LogWarning("Order with Id: {OrderId} cannot be cancelled because it's in status {Status}",
                    OrderId, orderStatus.Value.Name);

                return false;
            }

            await StateManager.SetStateAsync(OrderStatusStateName, OrderStatus.Cancelled);

            var order = await StateManager.GetStateAsync<Order>(OrderDetailsStateName);

            await _eventBus.PublishAsync(new OrderStatusChangedToCancelledIntegrationEvent(
                OrderId,
                OrderStatus.Cancelled.Name,
                $"The order was cancelled by buyer.",
                order.UserName));

            return true;
        }

        public async Task<bool> Ship()
        {
            var statusChanged = await TryUpdateOrderStatusAsync(OrderStatus.Paid, OrderStatus.Shipped);
            if (statusChanged)
            {
                var order = await StateManager.GetStateAsync<Order>(OrderDetailsStateName);

                await _eventBus.PublishAsync(new OrderStatusChangedToShippedIntegrationEvent(
                    OrderId,
                    OrderStatus.Shipped.Name,
                    "The order was shipped.",
                    order.UserName));

                return true;
            }

            return false;
        }

        public Task<Order> GetOrderDetails()
        {
            return StateManager.GetStateAsync<Order>(OrderDetailsStateName); 
        }

        public Task ReceiveReminderAsync(string reminderName, byte[] state, TimeSpan dueTime, TimeSpan period)
        {
            _logger.LogInformation("Received {Actor}[{ActorId}] reminder: {Reminder}", nameof(OrderingProcessActor), OrderId, reminderName);

            switch (reminderName)
            {
                case GracePeriodElapsedReminder: return OnGracePeriodElapsed();
                case StockConfirmedReminder: return OnStockConfirmedSimulatedWorkDone();
                case StockRejectedReminder:
                    {
                        var rejectedProductIds = JsonConvert.DeserializeObject<List<int>>(Encoding.UTF8.GetString(state));
                        return OnStockRejectedSimulatedWorkDone(rejectedProductIds);
                    }
                case PaymentSucceededReminder: return OnPaymentSucceededSimulatedWorkDone();
                case PaymentFailedReminder: return OnPaymentFailedSimulatedWorkDone();
            }

            _logger.LogError("Unknown actor reminder {ReminderName}", reminderName);
            return Task.CompletedTask;
        }

        public async Task OnGracePeriodElapsed()
        {
            var statusChanged = await TryUpdateOrderStatusAsync(OrderStatus.Submitted, OrderStatus.AwaitingStockValidation);
            if (statusChanged)
            {
                var order = await StateManager.GetStateAsync<Order>(OrderDetailsStateName);

                await _eventBus.PublishAsync(new OrderStatusChangedToAwaitingStockValidationIntegrationEvent(
                    OrderId,
                    OrderStatus.AwaitingStockValidation.Name,
                    "Grace period elapsed; waiting for stock validation.",
                    order.UserName,
                    order.OrderItems
                        .Select(orderItem => new OrderStockItem(orderItem.ProductId, orderItem.Units))));
            }
        }

        public async Task OnStockConfirmedSimulatedWorkDone()
        {
            var order = await StateManager.GetStateAsync<Order>(OrderDetailsStateName);

            await _eventBus.PublishAsync(new OrderStatusChangedToValidatedIntegrationEvent(
                OrderId,
                OrderStatus.Validated.Name,
                "All the items were confirmed with available stock.",
                order.UserName,
                order.GetTotal()));
        }

        public async Task OnStockRejectedSimulatedWorkDone(List<int> rejectedProductIds)
        {
            var order = await StateManager.GetStateAsync<Order>(OrderDetailsStateName);

            var rejectedProductNames = order.OrderItems
                .Where(orderItem => rejectedProductIds.Contains(orderItem.ProductId))
                .Select(orderItem => orderItem.ProductName);

            var rejectedDescription = string.Join(", ", rejectedProductNames);

            await _eventBus.PublishAsync(new OrderStatusChangedToCancelledIntegrationEvent(
                OrderId,
                OrderStatus.Cancelled.Name,
                $"The following product items don't have stock: ({rejectedDescription}).",
                order.UserName));
        }

        public async Task OnPaymentSucceededSimulatedWorkDone()
        {
            var order = await StateManager.GetStateAsync<Order>(OrderDetailsStateName);

            await _eventBus.PublishAsync(new OrderStatusChangedToPaidIntegrationEvent(
                OrderId,
                OrderStatus.Paid.Name,
                "The payment was performed at a simulated \"American Bank checking bank account ending on XX35071\"",
                order.UserName,
                order.OrderItems
                    .Select(orderItem => new OrderStockItem(orderItem.ProductId, orderItem.Units))));
        }

        public async Task OnPaymentFailedSimulatedWorkDone()
        {
            var order = await StateManager.GetStateAsync<Order>(OrderDetailsStateName);

            await _eventBus.PublishAsync(new OrderStatusChangedToCancelledIntegrationEvent(
                OrderId,
                OrderStatus.Cancelled.Name,
                "The order was cancelled because payment failed.",
                order.UserName));
        }

        protected override async Task OnPreActorMethodAsync(ActorMethodContext actorMethodContext)
        {
            var orderStatus = await StateManager.TryGetStateAsync<OrderStatus>(OrderStatusStateName);

            _preMethodOrderStatusId = orderStatus.HasValue ? orderStatus.Value.Id : (int?)null;
        }

        protected override async Task OnPostActorMethodAsync(ActorMethodContext actorMethodContext)
        {
            var postMethodOrderStatus = await StateManager.GetStateAsync<OrderStatus>(OrderStatusStateName);

            if (_preMethodOrderStatusId != postMethodOrderStatus.Id)
            {
                _logger.LogInformation("Order with Id: {OrderId} has been updated to status {Status}",
                    OrderId, postMethodOrderStatus.Name);
            }
        }

        private async Task<bool> TryUpdateOrderStatusAsync(OrderStatus expectedOrderStatus, OrderStatus newOrderStatus)
        {
            var orderStatus = await StateManager.TryGetStateAsync<OrderStatus>(OrderStatusStateName);
            if (!orderStatus.HasValue)
            {
                _logger.LogWarning("Order with Id: {OrderId} cannot be updated because it doesn't exist",
                    OrderId);

                return false;
            }

            if (orderStatus.Value.Id != expectedOrderStatus.Id)
            {
                _logger.LogWarning("Order with Id: {OrderId} is in status {Status} instead of expected status {ExpectedStatus}",
                    OrderId, orderStatus.Value.Name, expectedOrderStatus.Name);

                return false;
            }

            await StateManager.SetStateAsync(OrderStatusStateName, newOrderStatus);

            return true;
        }
    }
}
