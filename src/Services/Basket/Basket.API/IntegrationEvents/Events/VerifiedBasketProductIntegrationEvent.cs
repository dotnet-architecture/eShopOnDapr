namespace Microsoft.eShopOnDapr.Services.Basket.API.IntegrationEvents.Events;

public record VerifiedBasketProductIntegrationEvent(string BuyerId, Product[] Products) : IntegrationEvent;

// Curently records won't deserialize within an IntegrationEvent - so we'll have to use a class instead
//public record Product(int Id, string Name, decimal Price, string PictureFileName);
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string PictureFileName { get; set; } = string.Empty;
}
