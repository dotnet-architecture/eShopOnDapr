namespace Ordering.FunctionalTests;

public class OrderingScenarios : IClassFixture<OrderingWebApplicationFactory>
{
    private readonly OrderingWebApplicationFactory _factory;

    public OrderingScenarios(OrderingWebApplicationFactory factory)
        => _factory = factory;

    [Fact]
    public async Task Get_get_all_stored_orders_and_response_ok_status_code()
    {
        var response = await _factory.CreateClient()
            .GetAsync(Get.Orders);

        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Cancel_order_no_order_internal_server_error_response()
    {
        var response = await _factory.CreateIdempotentClient()
            .PutAsync(Put.CancelOrder(1), null);

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    public async Task Ship_order_no_order_internal_server_error_response()
    {
        var response = await _factory.CreateIdempotentClient()
            .PutAsync(Put.ShipOrder(1), null);

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }
}