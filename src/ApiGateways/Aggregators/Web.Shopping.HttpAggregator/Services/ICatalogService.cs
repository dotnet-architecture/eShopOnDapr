namespace Microsoft.eShopOnDapr.Web.Shopping.HttpAggregator.Services;

public interface ICatalogService
{
    Task<IEnumerable<CatalogItem>?> GetCatalogItemsAsync(IEnumerable<int> ids);
}
