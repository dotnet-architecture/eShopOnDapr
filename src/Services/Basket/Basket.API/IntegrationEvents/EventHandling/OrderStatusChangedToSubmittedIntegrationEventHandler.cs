using Basket.API.IntegrationEvents.Events;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using Microsoft.eShopOnContainers.Services.Basket.API;
using Microsoft.eShopOnContainers.Services.Basket.API.Model;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using System;
using System.Threading.Tasks;

namespace Basket.API.IntegrationEvents.EventHandling
{
    public class OrderStatusChangedToSubmittedIntegrationEventHandler : IIntegrationEventHandler<OrderStatusChangedToSubmittedIntegrationEvent>
    {
        private readonly IBasketRepository _repository;
        private readonly ILogger<OrderStatusChangedToSubmittedIntegrationEventHandler> _logger;

        public OrderStatusChangedToSubmittedIntegrationEventHandler(
            IBasketRepository repository,
            ILogger<OrderStatusChangedToSubmittedIntegrationEventHandler> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(OrderStatusChangedToSubmittedIntegrationEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{Program.AppName}"))
            {
                _logger.LogInformation("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);

                await _repository.DeleteBasketAsync(@event.BuyerId);
            }
        }
    }
}



