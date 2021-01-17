using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.eShopOnContainers.Services.Ordering.API.Model
{
    public class OrderStatus
    {
        public static readonly OrderStatus Submitted = new OrderStatus(1, nameof(Submitted));
        public static readonly OrderStatus AwaitingStockValidation = new OrderStatus(2, nameof(AwaitingStockValidation));
        public static readonly OrderStatus Validated = new OrderStatus(3, nameof(Validated));
        public static readonly OrderStatus Paid = new OrderStatus(4, nameof(Paid));
        public static readonly OrderStatus Shipped = new OrderStatus(5, nameof(Shipped));
        public static readonly OrderStatus Cancelled = new OrderStatus(6, nameof(Cancelled));

        private static readonly IEnumerable<OrderStatus> All
            = new[] { Submitted, AwaitingStockValidation, Validated, Paid, Shipped, Cancelled };

        public OrderStatus(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public static OrderStatus FromName(string name)
        {
            var state = All
                .SingleOrDefault(s => string.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new ArgumentException($"Possible values: {string.Join(",", All.Select(s => s.Name))}");
            }

            return state;
        }

        public static OrderStatus From(int id)
        {
            var state = All
                .SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new ArgumentException($"Possible values: {string.Join(",", All.Select(s => s.Id))}");
            }

            return state;
        }
    }
}
