using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.eShopOnContainers.Services.Ordering.API.Model;

namespace Microsoft.eShopOnContainers.Services.Ordering.API.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderingContext _orderingContext;

        public OrderRepository(OrderingContext orderingContext)
        {
            _orderingContext = orderingContext ?? throw new ArgumentNullException(nameof(orderingContext));

            _orderingContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public async Task<Order> GetOrAddOrderAsync(Order order)
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
                return await _orderingContext.Orders
                    .Where(o => o.RequestId == order.RequestId)
                    .Include(o => o.OrderStatus)
                    .Include(o => o.OrderItems)
                    .SingleOrDefaultAsync();
            }
        }

        public Task UpdateOrderAsync(Order order)
        {
            _orderingContext.Update(order);
            return _orderingContext.SaveChangesAsync();
        }

        public Task<Order> GetOrderAsync(int orderId)
        {
            return _orderingContext.Orders
                .Where(o => o.Id == orderId)
                .Include(o => o.OrderStatus)
                .Include(o => o.OrderItems)
                .SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<OrderSummary>> GetOrdersFromBuyerAsync(string buyerId)
        {
            return await _orderingContext.Orders
                .Where(o => o.BuyerId == buyerId)
                .Select(o => new OrderSummary
                {
                    Id = o.Id,
                    OrderDate = o.OrderDate,
                    OrderStatus = o.OrderStatus,
                    Total = o.GetTotal()
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<CardType>> GetCardTypesAsync()
        {
            return await _orderingContext.CardTypes
                .ToListAsync();
        }
    }
}
