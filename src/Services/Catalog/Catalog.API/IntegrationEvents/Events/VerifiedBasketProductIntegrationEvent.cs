namespace Microsoft.eShopOnDapr.Services.Catalog.API.IntegrationEvents.Events;

public record VerifiedBasketProductIntegrationEvent(string BuyerId, Product[] Products) : IntegrationEvent;

public record Product(int Id, string Name, decimal Price, string PictureFileName);
