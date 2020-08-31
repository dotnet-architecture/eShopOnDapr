namespace Ordering.Domain.Events
{
    using MediatR;

    /// <summary>
    /// Event used when the order stock items are confirmed
    /// </summary>
    public class OrderStatusChangedToValidatedDomainEvent
        : INotification
    {
        public int OrderId { get; }

        public OrderStatusChangedToValidatedDomainEvent(int orderId)
            => OrderId = orderId;
    }
}