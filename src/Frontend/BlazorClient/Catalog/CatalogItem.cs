using System.Collections.Generic;

namespace eShopOnDapr.BlazorClient.Catalog
{
    public class CatalogItem
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public string PictureUri { get; set; }

        public string GetFormattedPrice() => Price.ToString("0.00");
    }
}