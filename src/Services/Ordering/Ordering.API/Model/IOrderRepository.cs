using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.Services.Ordering.API.Model
{
    public interface IOrderRepository
    {
        Task<Order> GetOrderAsync(int orderId);
        Task<Order> GetOrAddOrderAsync(Order order);
        Task UpdateOrderAsync(Order order);
        Task<IEnumerable<OrderSummary>> GetOrdersFromBuyerAsync(string buyerId);
        Task<IEnumerable<CardType>> GetCardTypesAsync();
    }
}