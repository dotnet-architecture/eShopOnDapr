namespace Microsoft.eShopOnDapr.Web.Shopping.HttpAggregator.Models;

public record BasketDataItem(int ProductId, string ProductName, decimal UnitPrice, int Quantity, string PictureFileName);
