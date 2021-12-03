namespace Microsoft.eShopOnDapr.BlazorClient.Catalog;

public record CatalogPage(
    int Count,
    int PageIndex,
    int PageSize,
    IEnumerable<CatalogItem> Items)
{
    public int PageCount => (int)Math.Ceiling(Count / (decimal)PageSize);
}
