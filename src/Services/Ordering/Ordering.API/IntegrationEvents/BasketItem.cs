namespace Microsoft.eShopOnDapr.Services.Ordering.API.IntegrationEvents;

public class BasketItem
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public string PictureFileName { get; set; } = string.Empty;
}
