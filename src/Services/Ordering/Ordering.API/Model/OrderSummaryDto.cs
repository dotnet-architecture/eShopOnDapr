using System;

namespace Microsoft.eShopOnContainers.Services.Ordering.API.Model
{
    public class OrderSummaryDto
    {
        public int ordernumber { get; set; }
        public DateTime date { get; set; }
        public string status { get; set; }
        public decimal total { get; set; }

        public static OrderSummaryDto FromOrderSummary(OrderSummary orderSummary)
        {
            return new OrderSummaryDto
            {
                ordernumber = orderSummary.Id,
                date = orderSummary.OrderDate,
                status = orderSummary.OrderStatus.Name,
                total = orderSummary.Total
            };
        }
    }
}
