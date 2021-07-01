using System;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace eShopOnDapr.BlazorClient
{
    public class ApplicationAuthenticationState : RemoteAuthenticationState
    {
        public ApplicationAuthenticationState()
        {
        }

        public string Id { get; set; }
    }
}
