namespace eShopOnDapr.BlazorClient.Ordering
{
    public record OrderItem(
        int ProductId,
        string ProductName,
        decimal UnitPrice,
        int Units,
        string PictureUrl)
    {
        public decimal Total => UnitPrice * Units;
    }
}