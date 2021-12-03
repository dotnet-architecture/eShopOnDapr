namespace Microsoft.eShopOnDapr.Services.Ordering.API.Model;

public record OrderSummary(
    Guid Id,
    int OrderNumber,
    DateTime OrderDate,
    string OrderStatus,
    decimal Total);
