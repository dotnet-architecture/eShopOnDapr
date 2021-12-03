namespace Microsoft.eShopOnDapr.Services.Ordering.API.IntegrationEvents;

public record OrderStatusChangedToShippedIntegrationEvent(
    Guid OrderId,
    string OrderStatus,
    string Description,
    string BuyerId)
    : IntegrationEvent;
