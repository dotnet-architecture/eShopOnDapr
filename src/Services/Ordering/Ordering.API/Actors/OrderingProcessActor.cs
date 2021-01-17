using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapr.Actors;
using Dapr.Actors.Runtime;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using Microsoft.eShopOnContainers.Services.Ordering.API.Model;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Ordering.API.Application.IntegrationEvents.Events;

namespace Microsoft.eShopOnContainers.Services.Ordering.API.Actors
{
    public interface IOrderingProcessActor : IActor
    {
        Task Start(int buyerId, string buyerName, int paymentMethodId);

        Task NotifyStockConfirmed();

        Task NotifyStockRejected(List<int> rejectedProductIds);

        Task NotifyPaymentSucceeded();

        Task NotifyPaymentFailed();

        Task<bool> Cancel();

        Task<bool> Ship();
    }

    public interface IOrderReadModel
    {
        Task<Order> UpdateOrderAsync(int orderId, Action<Order> updateOrder);
    }

    public class OrderingProcessState
    {
        public int OrderStatusId { get; set; }
        public int BuyerId { get; set; }
        public string BuyerName { get; set; }
        public int PaymentMethodId { get; set; }
    }

    public class OrderingProcessActor : Actor, IOrderingProcessActor//, IRemindable
    {
        private const string OrderStatusStateName = "OrderStatus";

        private readonly IOrderRepository _orderRepository;
        private readonly IOrderReadModel _readModel;
        private readonly IEventBus _eventBus;
        private readonly ILogger<OrderingProcessActor> _logger;

        private int? _preMethodOrderStatusId;

        public OrderingProcessActor(ActorHost host) : base(host)
        {
        }

        private int OrderId => int.Parse(Id.GetId());

        public async Task Start(int buyerId, string buyerName, int paymentMethodId)
        {
            var processState = new OrderingProcessState
            {
                OrderStatusId = OrderStatus.Submitted.Id,
                BuyerId = buyerId,
                BuyerName = buyerName,
                PaymentMethodId = paymentMethodId
            };

            await StateManager.SetStateAsync("ProcessState", processState);

            await RegisterReminderAsync("GracePeriodConfirmed", null, TimeSpan.FromSeconds(10), TimeSpan.FromMilliseconds(-1));

            // Add Integration event to clean the basket
            // TODO Let subscriber (Basket) use OrderStatusChangedToSubmittedIntegrationEvent
            //var orderStartedIntegrationEvent = new OrderStartedIntegrationEvent(integrationEvent.UserId);
            //await _orderingIntegrationEventService.AddAndSaveEventAsync(orderStartedIntegrationEvent);
            await _eventBus.PublishAsync(new OrderStatusChangedToSubmittedIntegrationEvent(
                OrderId,
                OrderStatus.Submitted.Name,
                processState.BuyerName));

        }

        public async Task NotifyStockConfirmed()
        {
            var statusChanged = await TryUpdateOrderStatusAsync(OrderStatus.AwaitingStockValidation, OrderStatus.Validated);
            if (statusChanged)
            {
                // Simulate a work time for validating the payment.
                await RegisterReminderAsync(
                    "StockConfirmed",
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

                // Simulate a work time for validating the payment.
                await RegisterReminderAsync(
                    "StockRejected",
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
                // Simulate a work time for validating the payment.
                await RegisterReminderAsync("PaymentSucceeded", null, TimeSpan.FromSeconds(10), TimeSpan.FromMilliseconds(-1));
            }
        }

        public async Task NotifyPaymentFailed()
        {
            var statusChanged = await TryUpdateOrderStatusAsync(OrderStatus.Validated, OrderStatus.Paid);
            if (statusChanged)
            {
                // Simulate a work time for validating the payment.
                await RegisterReminderAsync("PaymentSucceeded", null, TimeSpan.FromSeconds(10), TimeSpan.FromMilliseconds(-1));
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

            var order = await UpdateReadModelAsync((order) =>
            {
                order.OrderStatus = OrderStatus.Cancelled;
                order.Description = $"The order was cancelled by buyer.";
            });

            await _eventBus.PublishAsync(new OrderStatusChangedToCancelledIntegrationEvent(
                order.Id,
                OrderStatus.Cancelled.Name,
                order.BuyerName));

            return true;
        }

        public async Task<bool> Ship()
        {
            var statusChanged = await TryUpdateOrderStatusAsync(OrderStatus.Paid, OrderStatus.Shipped);
            if (statusChanged)
            {
                var order = await UpdateReadModelAsync((order) =>
                {
                    order.OrderStatus = OrderStatus.Shipped;
                    order.Description = $"The order was shipped.";
                });

                await _eventBus.PublishAsync(new OrderStatusChangedToShippedIntegrationEvent(
                    order.Id,
                    OrderStatus.Shipped.Name,
                    order.BuyerName));

                return true;
            }

            return false;
        }

        //public async Task ReceiveReminderAsync(string reminderName, byte[] state, TimeSpan dueTime, TimeSpan period)
        //{
        //    _logger.LogDebug($"Reminder '{reminderName}' fired for actor '{Id.GetId()}'.");

        //    if (reminderName == REMINDER_GRACE_PERIOD_EXPIRED)
        //    {
        //        var orderId = await StateManager.GetStateAsync<int>("orderId");

        //        var order = await _orderRepository.GetAsync(orderId);
        //        order.status = "...";

        //        _orderRepository.Update(order);

        //        await _eventBus.PublishAsync(new OrderStatusChangedToAwaitingStockValidationIntegrationEvent());
        //    }
        //}

        public async Task OnGracePeriodConfirmed()
        {
            var statusChanged = await TryUpdateOrderStatusAsync(OrderStatus.Submitted, OrderStatus.AwaitingStockValidation);
            if (statusChanged)
            {
                var order = await UpdateReadModelAsync((order) =>
                {
                    order.OrderStatus = OrderStatus.AwaitingStockValidation;
                });

                await _eventBus.PublishAsync(new OrderStatusChangedToAwaitingStockValidationIntegrationEvent(
                    order.Id,
                    OrderStatus.AwaitingStockValidation.Name,
                    order.BuyerName,
                    order.OrderItems
                        .Select(orderItem => new OrderStockItem(orderItem.ProductId, orderItem.Units))));
            }
        }


        public async Task OnStockConfirmedSimulatedWorkDone()
        {
            var order = await UpdateReadModelAsync((order) =>
            {
                order.OrderStatus = OrderStatus.Validated;
                order.Description = "All the items were confirmed with available stock.";
            });

            await _eventBus.PublishAsync(new OrderStatusChangedToValidatedIntegrationEvent(
                order.Id,
                OrderStatus.Validated.Name,
                order.BuyerName,
                order.GetTotal()));
        }


        public async Task OnStockRejectedSimulatedWorkDone(List<int> rejectedProductIds)
        {
            var order = await UpdateReadModelAsync((order) =>
            {
                var rejectedProductNames = order.OrderItems
                    .Where(orderItem => rejectedProductIds.Contains(orderItem.ProductId))
                    .Select(orderItem => orderItem.ProductName);

                var rejectedDescription = string.Join(", ", rejectedProductNames);

                order.OrderStatus = OrderStatus.Cancelled;
                order.Description = $"The product items don't have stock: ({rejectedDescription}).";
            });

            await _eventBus.PublishAsync(new OrderStatusChangedToCancelledIntegrationEvent(
                order.Id,
                OrderStatus.Cancelled.Name,
                order.BuyerName));
        }

        public async Task OnPaymentSucceededSimulatedWorkDone()
        {
            var order = await UpdateReadModelAsync((order) =>
            {
                order.OrderStatus = OrderStatus.Paid;
                order.Description = "The payment was performed at a simulated \"American Bank checking bank account ending on XX35071\"";
            });

            await _eventBus.PublishAsync(new OrderStatusChangedToPaidIntegrationEvent(
                order.Id,
                OrderStatus.Paid.Name,
                order.BuyerName,
                order.OrderItems
                    .Select(orderItem => new OrderStockItem(orderItem.ProductId, orderItem.Units))));
        }

        public async Task OnPaymentFailedSimulatedWorkDone()
        {
            var order = await UpdateReadModelAsync((order) =>
            {
                order.OrderStatus = OrderStatus.Cancelled;
                order.Description = "The order was cancelled because payment failed.";
            });

            await _eventBus.PublishAsync(new OrderStatusChangedToCancelledIntegrationEvent(
                order.Id,
                OrderStatus.Cancelled.Name,
                order.BuyerName));
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

        private async Task<Order> UpdateReadModelAsync(Action<Order> updateOrder)
        {
            // TODO
            throw new NotImplementedException();
        }
    }
}
