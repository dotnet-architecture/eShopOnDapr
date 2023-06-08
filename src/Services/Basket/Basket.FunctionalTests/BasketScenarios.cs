namespace Basket.FunctionalTests;

public class BasketScenarios : IClassFixture<BasketWebApplicationFactory>
{
    private readonly BasketWebApplicationFactory _factory;

    public BasketScenarios(BasketWebApplicationFactory factory)
        => _factory = factory;

    [Fact]
    public async Task Post_basket_and_response_ok_status_code()
    {
        var content = new StringContent(BuildBasket(), Encoding.UTF8, "application/json");
        var response = await _factory.CreateClient()
            .PostAsync(Post.Basket, content);

        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Get_basket_and_response_ok_status_code()
    {
        var response = await _factory.CreateClient()
            .GetAsync(Get.Basket);

        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Send_Checkout_basket_and_response_ok_status_code()
    {
        var contentBasket = new StringContent(BuildBasket(), Encoding.UTF8, "application/json");

        await _factory.CreateClient()
            .PostAsync(Post.Basket, contentBasket);

        var contentCheckout = new StringContent(BuildCheckout(), Encoding.UTF8, "application/json");

        var response = await _factory.CreateIdempotentClient()
            .PostAsync(Post.CheckoutOrder, contentCheckout);

        response.EnsureSuccessStatusCode();
    }

    private string BuildBasket()
    {
        CustomerBasket order = new(AutoAuthorizeMiddleware.IDENTITY_ID);

        order.Items.Add(new BasketItem
        {
            ProductId = 1,
            ProductName = ".NET Bot Black Hoodie",
            UnitPrice = 10,
            Quantity = 1
        });

        return JsonSerializer.Serialize(order);
    }

    private string BuildCheckout()
    {
        BasketCheckout checkoutBasket = new(
            "buyer@email.com",
            "city",
            "street",
            "state",
            "coutry",
            "1234567890123456",
            "CardHolderName",
            DateTime.UtcNow.AddDays(1),
            "123");

        return JsonSerializer.Serialize(checkoutBasket);
    }
}