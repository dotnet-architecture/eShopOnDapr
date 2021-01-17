using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.eShopOnContainers.Services.Ordering.API.Model
{
    public class Order
    {
        public int Id { get; set; }
        public Guid RequestId { get; set; }
        public DateTime OrderDate { get; set; }
        public string Description { get; set; }
        public Address Address { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public string BuyerId { get; set; }
        public string BuyerName { get; set; }
        public int PaymentMethodId { get; set; }
        public List<OrderItem> OrderItems { get; set; }

        public decimal GetTotal()
        {
            var result = OrderItems.Sum(o => o.Units * o.UnitPrice);

            return result < 0 ? 0 : result;
        }
    }
}
