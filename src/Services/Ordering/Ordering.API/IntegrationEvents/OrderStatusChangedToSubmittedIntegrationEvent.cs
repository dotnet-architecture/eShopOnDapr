namespace Microsoft.eShopOnDapr.Services.Ordering.API.IntegrationEvents;

public record OrderStatusChangedToSubmittedIntegrationEvent(
    Guid OrderId,
    string OrderStatus,
    string BuyerId,
    string BuyerEmail)
    : IntegrationEvent;
