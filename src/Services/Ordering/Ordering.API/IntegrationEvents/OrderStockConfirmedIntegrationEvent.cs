namespace Microsoft.eShopOnDapr.Services.Ordering.API.IntegrationEvents;

public record OrderStockConfirmedIntegrationEvent(
    Guid OrderId)
    : IntegrationEvent;