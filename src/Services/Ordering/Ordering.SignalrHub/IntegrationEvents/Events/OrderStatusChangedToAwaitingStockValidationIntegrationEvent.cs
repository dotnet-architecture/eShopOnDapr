using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;
using System.Collections.Generic;

namespace Ordering.SignalrHub.IntegrationEvents
{
    public class OrderStatusChangedToAwaitingStockValidationIntegrationEvent : IntegrationEvent
    {
        public int OrderId { get; set; }
        public string OrderStatus { get; set; }
        public string BuyerName { get; set; }

        public OrderStatusChangedToAwaitingStockValidationIntegrationEvent()
        {
        }

        public OrderStatusChangedToAwaitingStockValidationIntegrationEvent(int orderId, string orderStatus, string buyerName)
        {
            OrderId = orderId;
            OrderStatus = orderStatus;
            BuyerName = buyerName;
        }
    }
}
