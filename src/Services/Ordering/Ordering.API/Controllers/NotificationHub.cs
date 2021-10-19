using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Microsoft.eShopOnDapr.Services.Ordering.API.Controllers
{
    [Authorize]
    public class NotificationsHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, GetUserId());
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception ex)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, GetUserId());
            await base.OnDisconnectedAsync(ex);
        }

        private string GetUserId() => Context.User.Claims.First(c => c.Type == "sub").Value;

    }
}
