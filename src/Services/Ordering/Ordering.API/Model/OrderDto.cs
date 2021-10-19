using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.eShopOnDapr.Services.Ordering.API.Model
{
    public class OrderDto
    {
        public int ordernumber { get; set; }
        public DateTime date { get; set; }
        public string status { get; set; }
        public string description { get; set; }
        public string street { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public List<OrderItemDto> orderitems { get; set; }
        public decimal subtotal { get; set; }
        public decimal total { get; set; }

        public static OrderDto FromOrder(Order order)
        {
            return new OrderDto
            {
                ordernumber = order.OrderNumber,
                date = order.OrderDate,
                status = order.OrderStatus,
                description = order.Description,
                street = order.Address.Street,
                city = order.Address.City,
                country = order.Address.Country,
                orderitems = order.OrderItems
                    .Select(OrderItemDto.FromOrderItem)
                    .ToList(),
                subtotal = order.GetTotal(),
                total = order.GetTotal()
            };
        }
    }
}
