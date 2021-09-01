namespace eShopOnDapr.BlazorClient.Catalog
{
    public record CatalogItem(
        int Id,
        string Name,
        string Description,
        decimal Price,
        string PictureUri)
    {
        public string GetFormattedPrice() => Price.ToString("0.00");
    }
}