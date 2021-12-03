namespace Microsoft.eShopOnDapr.Services.Payment.API.IntegrationEvents.Events;

public record OrderPaymentSucceededIntegrationEvent(Guid OrderId) : IntegrationEvent;
