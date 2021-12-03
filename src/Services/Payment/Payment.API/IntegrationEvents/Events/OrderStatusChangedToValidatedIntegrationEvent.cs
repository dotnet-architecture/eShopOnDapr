namespace Microsoft.eShopOnDapr.Services.Payment.API.IntegrationEvents.Events;

public record OrderStatusChangedToValidatedIntegrationEvent(
    Guid OrderId,
    decimal Total)
    : IntegrationEvent;
