namespace Microsoft.eShopOnDapr.Services.Ordering.API.IntegrationEvents;

public record GracePeriodConfirmedIntegrationEvent(int OrderId) : IntegrationEvent;
