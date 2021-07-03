//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Security.Claims;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Components.Authorization;

//namespace eShopOnDapr.BlazorClient
//{
//    public class UserInfo
//    {
////        private readonly AuthenticationStateProvider _authenticationStateProvider;

//        private Dictionary<string, string> _claims;

//        //public UserInfo(AuthenticationStateProvider authenticationStateProvider)
//        //{
//        //    _authenticationStateProvider = authenticationStateProvider
//        //        ?? throw new ArgumentNullException(nameof(authenticationStateProvider));
//        //}

//        public string Id => _claims?["sub"];

//        public string Email => _claims?["email"];

//        public string City => _claims?["address_city"];

//        public string Country => _claims?["address_country"];

//        public string State => _claims?["address_state"];

//        public string Street => _claims?["address_street"];

//        public string CardNumber => _claims?["card_number"];

//        public string CardHolderName => _claims?["card_holder"];

//        public string CardExpirationDate => _claims?["card_expiration"];

//        public string CardSecurityNumber => _claims?["card_security_number"];

//        public async Task LoadUserInfoAsync()
//        {
//            //var authenticationState =
//            //    await _authenticationStateProvider.GetAuthenticationStateAsync();

//            //if (authenticationState.User.Identity.IsAuthenticated)
//            //{
//                //_claims = authenticationState.User.Claims
//                //    .ToDictionary(c => c.Type, c => c.Value);
//            //}
//        }

//        public Task OnLoggedInInitializeAsync(ClaimsPrincipal user)
//        {
//            _claims = user.Claims.ToDictionary(c => c.Type, c => c.Value);

//            return Task.CompletedTask;
//        }
//    }
//}
