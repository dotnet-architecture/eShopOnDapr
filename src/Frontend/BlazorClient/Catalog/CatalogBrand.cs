using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace eShopOnDapr.BlazorClient.Catalog
{
    public class CatalogType
    {
        public int Id { get; set; }


        [JsonPropertyName("type")]
        public string Name { get; set; }
    }
}