namespace Ordering.API.Application.DomainEventHandlers.OrderValidated
{
    using Domain.Events;
    using MediatR;
    using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.BuyerAggregate;
    using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.OrderAggregate;
    using Microsoft.Extensions.Logging;
    using Ordering.API.Application.IntegrationEvents;
    using Ordering.API.Application.IntegrationEvents.Events;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class OrderStatusChangedToValidatedDomainEventHandler
        : INotificationHandler<OrderStatusChangedToValidatedDomainEvent>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IBuyerRepository _buyerRepository;
        private readonly ILoggerFactory _logger;
        private readonly IOrderingIntegrationEventService _orderingIntegrationEventService;

        public OrderStatusChangedToValidatedDomainEventHandler(
            IOrderRepository orderRepository,
            IBuyerRepository buyerRepository,
            ILoggerFactory logger,
            IOrderingIntegrationEventService orderingIntegrationEventService)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _buyerRepository = buyerRepository ?? throw new ArgumentNullException(nameof(buyerRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _orderingIntegrationEventService = orderingIntegrationEventService;
        }

        public async Task Handle(OrderStatusChangedToValidatedDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            _logger.CreateLogger<OrderStatusChangedToValidatedDomainEventHandler>()
                .LogTrace("Order with Id: {OrderId} has been successfully updated to status {Status} ({Id})",
                    domainEvent.OrderId, nameof(OrderStatus.Validated), OrderStatus.Validated.Id);

            var order = await _orderRepository.GetAsync(domainEvent.OrderId);
            var buyer = await _buyerRepository.FindByIdAsync(order.GetBuyerId.Value.ToString());

            var integrationEvent = new OrderStatusChangedToValidatedIntegrationEvent(order.Id, order.OrderStatus.Name, buyer.Name, order.GetTotal());
            await _orderingIntegrationEventService.AddAndSaveEventAsync(integrationEvent);
        }
    }
}