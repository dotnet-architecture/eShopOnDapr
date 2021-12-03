namespace Microsoft.eShopOnDapr.Web.Shopping.HttpAggregator.Models;

public record UpdateBasketRequest(IEnumerable<UpdateBasketRequestItemData> Items);
