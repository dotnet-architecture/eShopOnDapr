namespace Microsoft.eShopOnDapr.Services.Ordering.API.Actors;

public class OrderItemState
{
    // int Id, TODO
    //int OrderId, TODO
    public int ProductId { get; set; } = -1;
    public string ProductName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; } = -1;
    public int Units { get; set; } = -1;
    public string PictureFileName { get; set; } = string.Empty;
}
