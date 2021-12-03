namespace Microsoft.eShopOnDapr.Services.Ordering.API.IntegrationEvents;

public record OrderPaymentSucceededIntegrationEvent(Guid OrderId) : IntegrationEvent;
