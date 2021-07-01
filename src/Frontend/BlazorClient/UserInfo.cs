using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;

namespace eShopOnDapr.BlazorClient.Ordering
{
    public class UserInfo
    {
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        private Dictionary<string, string> _claims;

        public UserInfo(AuthenticationStateProvider authenticationStateProvider)
        {
            _authenticationStateProvider = authenticationStateProvider
                ?? throw new ArgumentNullException(nameof(authenticationStateProvider));
        }

        public string BuyerId => _claims?["sub"];

        public async Task LoadUserInfoAsync()
        {
            var authenticationState =
                await _authenticationStateProvider.GetAuthenticationStateAsync();

            if (authenticationState.User.Identity.IsAuthenticated)
            {
                _claims = authenticationState.User.Claims
                    .ToDictionary(c => c.Type, c => c.Value);
            }
        }
    }
}
