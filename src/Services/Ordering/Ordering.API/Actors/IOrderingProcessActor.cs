using System.Collections.Generic;
using System.Threading.Tasks;
using Dapr.Actors;
using Microsoft.eShopOnContainers.Services.Ordering.API.Application.Models;

namespace Microsoft.eShopOnContainers.Services.Ordering.API.Actors
{
    public interface IOrderingProcessActor : IActor
    {
        Task SubmitAsync(
            string userId, string userName, string street, string city,
            string zipCode, string state, string country, CustomerBasket basket);

        Task NotifyStockConfirmedAsync();

        Task NotifyStockRejectedAsync(List<int> rejectedProductIds);

        Task NotifyPaymentSucceededAsync();

        Task NotifyPaymentFailedAsync();

        Task<bool> CancelAsync();

        Task<bool> ShipAsync();

        Task<Order> GetOrderDetails();
    }
}
