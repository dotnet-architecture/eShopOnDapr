namespace Microsoft.eShopOnDapr.Services.Ordering.API.Model;

public class OrderItem
{
    public int Id { get; set; }
    public Guid OrderId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal UnitPrice { get; set; }
    public int Units { get; set; }
    public string PictureFileName { get; set; }

    public OrderItem()
    {
        OrderId = Guid.Empty;
        ProductName = string.Empty;
        PictureFileName = string.Empty;
    }

    public OrderItem(OrderItemState state)
    {
        ProductId = state.ProductId;
        ProductName = state.ProductName;
        UnitPrice = state.UnitPrice;
        Units = state.Units;
        PictureFileName = state.PictureFileName;
    }
}
