namespace Microsoft.eShopOnDapr.Services.Ordering.API.Application.Models
{
    public class BasketItem
    {
        public string Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public string PictureFileName { get; set; }
    }
}
