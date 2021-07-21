namespace eShopOnDapr.BlazorClient.Basket
{
    public class BasketItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public string PictureUrl { get; set; }

        public string GetFormattedPrice() => UnitPrice.ToString("0.00");
        public string GetFormattedTotalPrice() => (UnitPrice * Quantity).ToString("0.00");
    }
}
