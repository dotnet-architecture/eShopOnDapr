//using System.Collections.Generic;
//using System.Net.Http;
//using System.Net.Http.Json;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.SignalR.Client;

//namespace eShopOnDapr.BlazorClient.Ordering
//{
//    public class SignalRClient
//    {
//        private HubConnection hubConnection;

//        private readonly HttpClient _httpClient;

//        public OrderClient(HttpClient httpClient)
//        {
//            _httpClient = httpClient;
//        }

//        public Task ConnectAsync()
//        {
//            hubConnection = new HubConnectionBuilder()
//            .WithUrl(NavigationManager.ToAbsoluteUri("/chathub"))
//            .Build();

//            hubConnection.On<string, string>("ReceiveMessage", (user, message) =>
//            {
//                var encodedMsg = $"{user}: {message}";
//                messages.Add(encodedMsg);
//                StateHasChanged();
//            });

//            await hubConnection.StartAsync();
//        }

//        hubConnection.State == HubConnectionState.Connected;

//    public async ValueTask DisposeAsync()
//        {
//            if (hubConnection is not null)
//            {
//                await hubConnection.DisposeAsync();
//            }
//        }
//    }
//}
