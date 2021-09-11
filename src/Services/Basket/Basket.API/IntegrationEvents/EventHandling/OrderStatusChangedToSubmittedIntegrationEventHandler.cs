using System;
using System.Threading.Tasks;
using Microsoft.eShopOnDapr.BuildingBlocks.EventBus.Abstractions;
using Microsoft.eShopOnDapr.Services.Basket.API.IntegrationEvents.Events;
using Microsoft.eShopOnDapr.Services.Basket.API.Model;

namespace Microsoft.eShopOnDapr.Services.Basket.API.IntegrationEvents.EventHandling
{
    public class OrderStatusChangedToSubmittedIntegrationEventHandler : IIntegrationEventHandler<OrderStatusChangedToSubmittedIntegrationEvent>
    {
        private readonly IBasketRepository _repository;

        public OrderStatusChangedToSubmittedIntegrationEventHandler(
            IBasketRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public Task Handle(OrderStatusChangedToSubmittedIntegrationEvent @event)
            => _repository.DeleteBasketAsync(@event.BuyerId);
    }
}



