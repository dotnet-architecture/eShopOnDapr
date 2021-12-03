namespace Microsoft.eShopOnDapr.Services.Ordering.API.IntegrationEvents;

public record OrderPaymentFailedIntegrationEvent(Guid OrderId) : IntegrationEvent;
