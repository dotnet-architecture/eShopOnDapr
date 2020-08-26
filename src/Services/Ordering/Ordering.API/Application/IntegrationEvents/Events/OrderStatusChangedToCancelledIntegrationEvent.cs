using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ordering.API.Application.IntegrationEvents.Events
{
    public class OrderStatusChangedToCancelledIntegrationEvent : IntegrationEvent
    {
        public int OrderId { get; }
        public string OrderStatus { get; }
        public string BuyerName { get; }
        public string DiscountCode { get; }

        public OrderStatusChangedToCancelledIntegrationEvent(int orderId, string orderStatus, string buyerName, string discountCode)
        {
            OrderId = orderId;
            OrderStatus = orderStatus;
            BuyerName = buyerName;
            DiscountCode = discountCode;
        }
    }
}
