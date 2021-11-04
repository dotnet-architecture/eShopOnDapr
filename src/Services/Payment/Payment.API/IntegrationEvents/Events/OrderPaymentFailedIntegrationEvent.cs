namespace Microsoft.eShopOnDapr.Services.Payment.API.IntegrationEvents.Events;

public record OrderPaymentFailedIntegrationEvent(Guid OrderId) : IntegrationEvent;
