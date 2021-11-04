namespace Microsoft.eShopOnDapr.BlazorClient.Catalog;

public record CatalogItem(
    int Id,
    string Name,
    decimal Price,
    string PictureFileName)
{
    public string GetFormattedPrice() => Price.ToString("0.00");

    public string GetPictureUrl(Settings settings) => $"{settings.ApiGatewayUrlExternal}/c/pics/{PictureFileName}";
}
