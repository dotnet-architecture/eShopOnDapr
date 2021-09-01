namespace eShopOnDapr.BlazorClient.Basket
{
    public record BasketItem(
        int ProductId,
        string ProductName,
        decimal UnitPrice,
        int Quantity,
        string PictureUrl)
    {
        public string GetFormattedPrice() => UnitPrice.ToString("0.00");
        public string GetFormattedTotalPrice() => (UnitPrice * Quantity).ToString("0.00");
    }
}