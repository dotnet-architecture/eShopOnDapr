namespace Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.OrderAggregate
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using global::Ordering.Domain.Exceptions;
    using Microsoft.eShopOnContainers.Services.Ordering.Domain.SeedWork;

    public class OrderStatus
        : Enumeration
    {
        public static readonly OrderStatus Submitted = new OrderStatus(1, nameof(Submitted));
        public static readonly OrderStatus AwaitingStockValidation = new OrderStatus(2, nameof(AwaitingStockValidation));
        public static readonly OrderStatus AwaitingCouponValidation = new OrderStatus(3, nameof(AwaitingCouponValidation));
        public static readonly OrderStatus Validated = new OrderStatus(4, nameof(Validated));
        public static readonly OrderStatus Paid = new OrderStatus(5, nameof(Paid));
        public static readonly OrderStatus Shipped = new OrderStatus(6, nameof(Shipped));
        public static readonly OrderStatus Cancelled = new OrderStatus(7, nameof(Cancelled));

        public OrderStatus(int id, string name)
            : base(id, name)
        {
        }

        public static IEnumerable<OrderStatus> List() =>
            new[] { Submitted, AwaitingStockValidation, Validated, AwaitingCouponValidation, Paid, Shipped, Cancelled };

        public static OrderStatus FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new OrderingDomainException($"Possible values for OrderStatus: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public static OrderStatus From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new OrderingDomainException($"Possible values for OrderStatus: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }
    }
}
