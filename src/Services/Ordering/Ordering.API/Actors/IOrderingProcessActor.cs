using System.Collections.Generic;
using System.Threading.Tasks;
using Dapr.Actors;
using Microsoft.eShopOnDapr.Services.Ordering.API.Application.Models;

namespace Microsoft.eShopOnDapr.Services.Ordering.API.Actors
{
    public interface IOrderingProcessActor : IActor
    {
        Task SubmitAsync(
            string buyerId, string buyerEmail, string street, string city, string state,
            string country, CustomerBasket basket);

        Task NotifyStockConfirmedAsync();

        Task NotifyStockRejectedAsync(List<int> rejectedProductIds);

        Task NotifyPaymentSucceededAsync();

        Task NotifyPaymentFailedAsync();

        Task<bool> CancelAsync();

        Task<bool> ShipAsync();

        Task<Order> GetOrderDetails();
    }
}
