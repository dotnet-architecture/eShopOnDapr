namespace Microsoft.eShopOnDapr.Web.Shopping.HttpAggregator.Services;

public class BasketService : IBasketService
{
    private readonly HttpClient _httpClient;

    public BasketService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task UpdateAsync(BasketData currentBasket, string accessToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "api/v1/basket")
        {
            Content = JsonContent.Create(currentBasket)
        };

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }
}

