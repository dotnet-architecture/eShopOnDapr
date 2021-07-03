using System;
using System.Collections.Generic;
using System.Linq;

namespace eShopOnDapr.BlazorClient.Ordering
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
    }
}
