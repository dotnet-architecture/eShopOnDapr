namespace Microsoft.eShopOnDapr.Services.Ordering.API.IntegrationEvents;

public record ConfirmedOrderStockItem(int ProductId, bool HasStock);
