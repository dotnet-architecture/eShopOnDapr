namespace Microsoft.eShopOnDapr.Services.Ordering.API.IntegrationEvents;

public record OrderStatusChangedToValidatedIntegrationEvent(
    Guid OrderId,
    string OrderStatus,
    string Description,
    decimal Total,
    string BuyerId)
    : IntegrationEvent;