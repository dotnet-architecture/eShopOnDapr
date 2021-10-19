using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.eShopOnDapr.Services.Ordering.API.Actors
{
    public class Order
    {
        public DateTime OrderDate { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public string Description { get; set; }
        public OrderAddress Address { get; set; }
        public string BuyerId { get; set; }
        public string BuyerEmail { get; set; }
        public List<OrderItem> OrderItems { get; set; }

        public decimal GetTotal()
        {
            var result = OrderItems.Sum(o => o.Units * o.UnitPrice);

            return result < 0 ? 0 : result;
        }
    }
}