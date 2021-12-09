namespace Microsoft.eShopOnDapr.Services.Basket.API.Model;

public class CustomerBasket
{
    public string BuyerId { get; set; } = "";

    public List<BasketItem> Items { get; set; } = new List<BasketItem>();

    public CustomerBasket()
    {

    }

    public CustomerBasket(string buyerId)
    {
        BuyerId = buyerId;
    }

    public decimal GetTotal() => Items.Sum(o => o.Quantity * o.UnitPrice);

    public bool IsVerified => Items.All(i => i.IsVerified);
}
