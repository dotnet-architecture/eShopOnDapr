using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.eShopOnDapr.Services.Ordering.API.Model
{
    public class Order
    {
        public Guid Id { get; set; }
        public int OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderStatus { get; set; }
        public string Description { get; set; }
        public Address Address { get; set; }
        public string BuyerId { get; set; }
        public string BuyerEmail { get; set; }
        public List<OrderItem> OrderItems { get; set; }

        public decimal GetTotal()
        {
            var result = OrderItems.Sum(o => o.Units * o.UnitPrice);

            return result < 0 ? 0 : result;
        }

        public static Order FromActorState(Guid orderId, Actors.Order order)
        {
            return new Order
            {
                Id = orderId,
                OrderDate = order.OrderDate,
                OrderStatus = order.OrderStatus.Name,
                BuyerId = order.BuyerId,
                BuyerEmail = order.BuyerEmail,
                Address = new Address
                {
                    Street = order.Address.Street,
                    City = order.Address.City,
                    State = order.Address.State,
                    Country = order.Address.Country
                },
                OrderItems = order.OrderItems
                    .Select(OrderItem.FromActorState)
                    .ToList()
            };
        }
    }
}
