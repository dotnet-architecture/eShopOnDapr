namespace Microsoft.eShopOnDapr.Web.Shopping.HttpAggregator.Models;

public class BasketData
{
    public List<BasketDataItem> Items { get; set; } = new List<BasketDataItem>();
}
