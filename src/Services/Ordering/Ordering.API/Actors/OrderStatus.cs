namespace Microsoft.eShopOnDapr.Services.Ordering.API.Actors;

public class OrderStatus
{
    public static readonly OrderStatus New = new OrderStatus(0, nameof(New));
    public static readonly OrderStatus Submitted = new OrderStatus(1, nameof(Submitted));
    public static readonly OrderStatus AwaitingStockValidation = new OrderStatus(2, nameof(AwaitingStockValidation));
    public static readonly OrderStatus Validated = new OrderStatus(3, nameof(Validated));
    public static readonly OrderStatus Paid = new OrderStatus(4, nameof(Paid));
    public static readonly OrderStatus Shipped = new OrderStatus(5, nameof(Shipped));
    public static readonly OrderStatus Cancelled = new OrderStatus(6, nameof(Cancelled));

    public int Id { get; set; }

    public string Name { get; set; }

    public OrderStatus()
        : this(New.Id, New.Name)
    {
    }

    public OrderStatus(int id, string name)
    {
        Id = id;
        Name = name;
    }
}
