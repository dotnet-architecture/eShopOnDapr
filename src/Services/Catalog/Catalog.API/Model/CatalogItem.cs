namespace Microsoft.eShopOnDapr.Services.Catalog.API.Model;

public class CatalogItem
{
    public CatalogItem(
        int id,
        string name,
        decimal price,
        string pictureFileName,
        int catalogTypeId,
        int catalogBrandId,
        int availableStock) 
    {
        Id = id;
        Name = name;
        Price = price;
        PictureFileName = pictureFileName;
        CatalogTypeId = catalogTypeId;
        CatalogBrandId = catalogBrandId;
        AvailableStock = availableStock;
    }

    public int Id { get; private set; }

    public string Name { get; set; }

    public decimal Price { get; set; }

    public string PictureFileName { get; set; }

    public int CatalogTypeId { get; set; }

    public CatalogType CatalogType { get; set; } = null!;

    public int CatalogBrandId { get; set; }

    public CatalogBrand CatalogBrand { get; set; } = null!;

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
