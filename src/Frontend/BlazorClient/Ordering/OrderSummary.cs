using System;

namespace eShopOnDapr.BlazorClient.Ordering
{
    public record OrderSummary(
        Guid Id,
        int OrderNumber,
        DateTime OrderDate,
        string OrderStatus,
        decimal Total);
}
