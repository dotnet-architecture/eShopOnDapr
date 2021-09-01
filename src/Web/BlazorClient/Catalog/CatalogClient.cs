using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace eShopOnDapr.BlazorClient.Catalog
{
    public class CatalogClient
    {
        private const int PageSize = 12;

        private readonly HttpClient httpClient;

        public CatalogClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<IEnumerable<CatalogBrand>> GetBrandsAsync() =>
            await httpClient.GetFromJsonAsync<IEnumerable<CatalogBrand>>(
                "c/api/v1/catalog/catalogbrands");

        public async Task<IEnumerable<CatalogType>> GetTypesAsync() =>
            await httpClient.GetFromJsonAsync<IEnumerable<CatalogType>>(
                "c/api/v1/catalog/catalogtypes");

        public async Task<CatalogPage> GetItemsAsync(int brandId, int typeId, int pageIndex) =>
            await httpClient.GetFromJsonAsync<CatalogPage>(
                $"c/api/v1/catalog/items2?brandId={brandId}&typeId={typeId}&pageIndex={pageIndex}&pageSize={PageSize}");
    }
}
