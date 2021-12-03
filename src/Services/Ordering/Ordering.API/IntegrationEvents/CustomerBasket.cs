namespace Microsoft.eShopOnDapr.Services.Ordering.API.IntegrationEvents;

public class CustomerBasket
{
    public string BuyerId { get; set; } = string.Empty;

    public List<BasketItem> Items { get; set; } = new();
}
