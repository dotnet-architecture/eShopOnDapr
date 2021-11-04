namespace Microsoft.eShopOnDapr.BlazorClient.Catalog;

public class CatalogClient
{
    private const int PageSize = 12;

    private readonly HttpClient httpClient;

    public CatalogClient(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public Task<IEnumerable<CatalogBrand>> GetBrandsAsync() =>
        httpClient.GetFromJsonAsync<IEnumerable<CatalogBrand>>(
            "c/api/v1/catalog/brands")!;

    public Task<IEnumerable<CatalogType>> GetTypesAsync() =>
        httpClient.GetFromJsonAsync<IEnumerable<CatalogType>>(
            "c/api/v1/catalog/types")!;

    public Task<CatalogPage> GetItemsAsync(int brandId, int typeId, int pageIndex) =>
        httpClient.GetFromJsonAsync<CatalogPage>(
            $"c/api/v1/catalog/items/by_page?brandId={brandId}&typeId={typeId}&pageIndex={pageIndex}&pageSize={PageSize}")!;
}
