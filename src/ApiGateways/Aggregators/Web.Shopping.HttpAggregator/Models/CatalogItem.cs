namespace Microsoft.eShopOnDapr.Web.Shopping.HttpAggregator.Models
{
    public class CatalogItem
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public string PictureFileName { get; set; }
    }
}
