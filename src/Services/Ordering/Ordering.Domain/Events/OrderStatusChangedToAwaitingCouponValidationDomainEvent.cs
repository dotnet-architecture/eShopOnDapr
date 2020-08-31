namespace Ordering.Domain.Events
{
    using MediatR;

    public class OrderStatusChangedToAwaitingCouponValidationDomainEvent : INotification
    {
        public int OrderId { get; }

        public string Code { get; set; }

        public OrderStatusChangedToAwaitingCouponValidationDomainEvent(int orderId, string code)
        {
            OrderId = orderId;
            Code = code;
        }
    }
}