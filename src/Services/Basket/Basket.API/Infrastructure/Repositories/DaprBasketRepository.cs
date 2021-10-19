using Dapr.Client;
using Microsoft.eShopOnDapr.Services.Basket.API.Model;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Microsoft.eShopOnDapr.Services.Basket.API.Infrastructure.Repositories
{
    public class DaprBasketRepository : IBasketRepository
    {
        private const string StoreName = "eshop-statestore";

        private readonly ILogger<DaprBasketRepository> _logger;
        private readonly DaprClient _dapr;

        public DaprBasketRepository(ILoggerFactory loggerFactory, DaprClient dapr)
        {
            _logger = loggerFactory.CreateLogger<DaprBasketRepository>();
            _dapr = dapr;
        }

        public async Task DeleteBasketAsync(string id)
        {
            await _dapr.DeleteStateAsync(StoreName, id);
        }

        public async Task<CustomerBasket> GetBasketAsync(string customerId)
        {
            return await _dapr.GetStateAsync<CustomerBasket>(StoreName, customerId);
        }

        public async Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket)
        {
            var state = await _dapr.GetStateEntryAsync<CustomerBasket>(StoreName, basket.BuyerId);
            state.Value = basket;

            await state.SaveAsync();

            _logger.LogInformation("Basket item persisted successfully.");

            return await GetBasketAsync(basket.BuyerId);
        }
    }
}