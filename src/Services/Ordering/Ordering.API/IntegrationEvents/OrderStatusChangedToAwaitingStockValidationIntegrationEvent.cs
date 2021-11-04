namespace Microsoft.eShopOnDapr.Services.Ordering.API.IntegrationEvents;

public record OrderStatusChangedToAwaitingStockValidationIntegrationEvent(
    Guid OrderId,
    string OrderStatus,
    string Description,
    IEnumerable<OrderStockItem> OrderStockItems,
    string BuyerId)
    : IntegrationEvent;
