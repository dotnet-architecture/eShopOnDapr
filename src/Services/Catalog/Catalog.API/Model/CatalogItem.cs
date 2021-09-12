namespace Microsoft.eShopOnDapr.Services.Catalog.API.Model
{
    public class CatalogItem
    {
        public CatalogItem() 
        {
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public string PictureFileName { get; set; }

        public int CatalogTypeId { get; set; }

        public CatalogType CatalogType { get; set; }

        public int CatalogBrandId { get; set; }

        public CatalogBrand CatalogBrand { get; set; }

        public int AvailableStock { get; set; }

        /// <summary>
        /// Simply decrement the quantity of a particular item in inventory.
        /// We don't care if we run out of stock.
        /// </summary>
        public int RemoveStock(int quantityDesired)
        {
            AvailableStock -= quantityDesired;

            return quantityDesired;
        }
    }
}