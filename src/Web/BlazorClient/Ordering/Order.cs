using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.eShopOnDapr.BlazorClient.Ordering
{
    public record Order(
        int OrderNumber,
        DateTime OrderDate,
        string OrderStatus,
        string Description,
        Address Address,
        List<OrderItem> OrderItems)
    {
        public decimal Total => OrderItems.Sum(o => o.Units * o.UnitPrice);

        public string GetFormattedOrderDate() => OrderDate.ToString("d");

        public string GetFormattedTotal() => Total.ToString("0.00");
    }
}
