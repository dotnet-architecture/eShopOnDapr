using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace eShopOnDapr.BlazorClient.Catalog
{
    public class CatalogBrand
    {
        public int Id { get; set; }


        [JsonPropertyName("brand")]
        public string Name { get; set; }
    }
}