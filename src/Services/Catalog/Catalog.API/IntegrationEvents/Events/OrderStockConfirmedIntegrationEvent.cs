namespace Microsoft.eShopOnDapr.Services.Catalog.API.IntegrationEvents.Events;

public record OrderStockConfirmedIntegrationEvent(Guid OrderId) : IntegrationEvent;
