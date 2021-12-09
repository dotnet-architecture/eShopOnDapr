namespace Microsoft.eShopOnDapr.BlazorClient.Basket;

public class BasketClient
{
    private readonly HttpClient _httpClient;

    public BasketClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<BasketData> GetBasketAsync()
    {
        var basket = await _httpClient.GetFromJsonAsync<BasketData>(
            "b/api/v1/basket/");

        return basket!;
    }

    public async Task<IEnumerable<BasketItem>> SaveItemsAsync(IEnumerable<BasketItem> items)
    {
        var request = new BasketData(items);

        var response = await _httpClient.PostAsJsonAsync(
            "b/api/v1/basket/",
            request);

        response.EnsureSuccessStatusCode();

        var basketData = await response.Content.ReadFromJsonAsync<BasketData>();
        return basketData!.Items;
    }

    public async Task CheckoutAsync(BasketCheckout basketCheckout)
    {
        var response = await _httpClient.PostAsJsonAsync(
            "b/api/v1/basket/checkout",
            basketCheckout);

        response.EnsureSuccessStatusCode();
    }
}
