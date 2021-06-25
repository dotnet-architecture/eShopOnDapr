//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net.Http;
//using System.Net.Http.Json;
//using System.Threading.Tasks;

//namespace eShopOnDapr.BlazorClient.Basket
//{
//    public class LocalStorageBasketStateProvider : IBasketStateStore
//    {
//        private readonly IJSRuntime _js;

//        public LocalStorageBasketStateProvider(IJSRuntime js)
//        {
//            _js = js ?? throw new ArgumentNullException(nameof(js));
//        }

//        public Task<IEnumerable<BasketItem>> LoadBasketItemsAsync()
//        {
//            var locallyStoredState = await _js.InvokeAsync<string>(
//                "sessionStorage.getItem", "eshop.basket");

//            if (locallyStoredState != null)
//            {


//                //Console.WriteLine("Restored state: " + locallyStoredState);
//                ////State.SetStateFromLocalStorage(locallyStoredState);
//                //await JS.InvokeVoidAsync("sessionStorage.removeItem", state.Id);
//            }



//            JsonSerializer.Serialize(this);
//            }

//            public void SetStateFromLocalStorage(string locallyStoredState)
//            {
//                var deserializedState =
//                    JsonSerializer.Deserialize<StateContainer>(locallyStoredState);

//                CounterValue = deserializedState.CounterValue;
//            }

//            throw new NotImplementedException();
//        }

//        public Task SaveBasketItemsAsync(IEnumerable<BasketItem> items)
//        {


//        await JS.InvokeVoidAsync("sessionStorage.setItem",
//            AuthenticationState.Id, "foo");
//    }
//}