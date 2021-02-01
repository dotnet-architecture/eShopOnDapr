using System.Collections.Generic;
using System.Threading.Tasks;
using Dapr.Actors;
using Microsoft.eShopOnContainers.Services.Ordering.API.Application.Models;

namespace Microsoft.eShopOnContainers.Services.Ordering.API.Actors
{
    public interface IOrderingProcessActor : IActor
    {
        Task Submit(string userId, string userName, string street, string city,
            string zipCode, string state, string country, CustomerBasket basket);

        Task NotifyStockConfirmed();

        Task NotifyStockRejected(List<int> rejectedProductIds);

        Task NotifyPaymentSucceeded();

        Task NotifyPaymentFailed();

        Task<bool> Cancel();

        Task<bool> Ship();

        Task<Order> GetOrderDetails();
    }
}
