namespace Microsoft.eShopOnDapr.Services.Ordering.API.Model;

public class Order
{
    public Guid Id { get; private set; }
    public int OrderNumber { get; private set; }
    public DateTime OrderDate { get; private set; }
    public string OrderStatus { get; set; }
    public string Description { get; set; }
    public Address Address { get; private set; }
    public string BuyerId { get; private set; }
    public string BuyerEmail { get; private set; }
    public List<OrderItem> OrderItems { get; private set; }

    public Order()
    {
        Id = Guid.Empty;
        OrderStatus = string.Empty;
        Description = string.Empty;
        Address = new Address();
        BuyerId = string.Empty;
        BuyerEmail = string.Empty;
        OrderItems = new();
    }

    public Order(Guid orderId, OrderState state)
    {
        Id = orderId;
        OrderDate = state.OrderDate;
        OrderStatus = state.OrderStatus.Name;
        BuyerId = state.BuyerId;
        BuyerEmail = state.BuyerEmail;
        Description = state.Description;
        Address = new Address(
            state.Address.Street,
            state.Address.City,
            state.Address.State,
            state.Address.Country);
        OrderItems = state.OrderItems
            .Select(itemState => new OrderItem(itemState))
            .ToList();
    }

    public decimal GetTotal() => OrderItems.Sum(o => o.Units * o.UnitPrice);
}
