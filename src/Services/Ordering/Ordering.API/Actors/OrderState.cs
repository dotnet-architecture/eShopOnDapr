namespace Microsoft.eShopOnDapr.Services.Ordering.API.Actors;

public class OrderState
{
    public DateTime OrderDate { get; set; }
    public OrderStatus OrderStatus { get; set; } = OrderStatus.New;
    public string Description { get; set; } = string.Empty;
    public OrderAddressState Address { get; set; } = new();
    public string BuyerId { get; set; } = string.Empty;
    public string BuyerEmail { get; set; } = string.Empty;
    public List<OrderItemState> OrderItems { get; set; } = new();

    public decimal GetTotal() => OrderItems.Sum(o => o.Units * o.UnitPrice);
}
