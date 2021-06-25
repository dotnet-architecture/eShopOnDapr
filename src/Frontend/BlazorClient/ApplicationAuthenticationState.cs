using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.Text.Json;

namespace eShopOnDapr.BlazorClient
{
    public class StateContainer
    {
        public int CounterValue { get; set; }

        public string GetStateForLocalStorage()
        {
            return JsonSerializer.Serialize(this);
        }

        public void SetStateFromLocalStorage(string locallyStoredState)
        {
            var deserializedState = 
                JsonSerializer.Deserialize<StateContainer>(locallyStoredState);

            CounterValue = deserializedState.CounterValue;
        }
    }

    public class ApplicationAuthenticationState : RemoteAuthenticationState
    {
        public string Id { get; set; }
    }
}