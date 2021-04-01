using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<CatalogPage> GetPage(int pageIndex) =>
            await httpClient.GetFromJsonAsync<CatalogPage>($"items?pageIndex={pageIndex}&pageSize={PageSize}");
    }
}
