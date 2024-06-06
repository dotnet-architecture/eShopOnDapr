using Microsoft.AspNetCore.Mvc.Testing;

namespace Catalog.FunctionalTests;

public class CatalogWebApplicationFactory : WebApplicationFactory<Program>
{
    private const string ApiUrlBase = $"api/v1/catalog";

    public static class Get
    {
        private const int PageIndex = 0;
        private const int PageCount = 4;

        public static string Items(bool paginated = false)
        {
            return paginated
                ? $"{ApiUrlBase}/items/by_page" + Paginated(PageIndex, PageCount)
                : $"{ApiUrlBase}/items/by_page";
        }

        public static string ItemByIds(params int[] ids)
        {
            return $"{ApiUrlBase}/items/by_ids?ids={string.Join(",", ids)}";
        }

        public static string Types = $"{ApiUrlBase}/types";

        public static string Brands = $"{ApiUrlBase}/brands";

        public static string Filtered(int catalogTypeId, int catalogBrandId, bool paginated = false)
        {
            return paginated
                ? $"{ApiUrlBase}/items/by_page" + Paginated(PageIndex, PageCount) + $"&typeId={catalogTypeId}&brandId={catalogBrandId}"
                : $"{ApiUrlBase}/items/by_page?typeId={catalogTypeId}&brandId={catalogBrandId}";
        }

        private static string Paginated(int pageIndex, int pageCount)
        {
            return $"?pageIndex={pageIndex}&pageSize={pageCount}";
        }
    }
}