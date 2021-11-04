namespace Microsoft.eShopOnDapr.Services.Ordering.API.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly OrderingDbContext _orderingContext;

    public OrderRepository(OrderingDbContext orderingContext)
    {
        _orderingContext = orderingContext ?? throw new ArgumentNullException(nameof(orderingContext));

        _orderingContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    public async Task<Order> AddOrGetOrderAsync(Order order)
    {
        _orderingContext.Add(order);

        try
        {
            await _orderingContext.SaveChangesAsync();
            return order;
        }
        catch (DbUpdateException ex)
            when ((ex.InnerException as SqlException)?.Number == 2627)
        {
            return (await GetOrderByIdAsync(order.Id))!;
        }
    }

    public Task UpdateOrderAsync(Order order)
    {
        _orderingContext.Update(order);

        return _orderingContext.SaveChangesAsync();
    }

    public Task<Order?> GetOrderByIdAsync(Guid orderId)
    {
        return _orderingContext.Orders
            .Where(o => o.Id == orderId)
            .Include(o => o.OrderItems)
            .SingleOrDefaultAsync();
    }

    public Task<Order?> GetOrderByOrderNumberAsync(int orderNumber)
    {
        return _orderingContext.Orders
            .Where(o => o.OrderNumber == orderNumber)
            .Include(o => o.OrderItems)
            .SingleOrDefaultAsync();
    }

    public async Task<IEnumerable<OrderSummary>> GetOrdersFromBuyerAsync(string buyerId)
    {
        return await _orderingContext.Orders
            .Where(o => o.BuyerId == buyerId)
            .Include(o => o.OrderItems)
            .Select(o => new OrderSummary(
                o.Id,
                o.OrderNumber,
                o.OrderDate,
                o.OrderStatus,
                o.GetTotal()))
            .ToListAsync();
    }
}
