namespace Microsoft.eShopOnDapr.Services.Ordering.API.IntegrationEvents;

public record OrderStatusChangedToCancelledIntegrationEvent(
    Guid OrderId,
    string OrderStatus,
    string Description,
    string BuyerId)
    : IntegrationEvent;
