extern alias basket;

using basket.Microsoft.eShopOnDapr.Services.Basket.API.Model;

namespace FunctionalTests.Services.Ordering;

public class IntegrationEventsScenarios : IClassFixture<OrderingWebApplicationFactory>, IClassFixture<BasketWebApplicationFactory>
{
    private readonly OrderingWebApplicationFactory _orderingWebFactory;
    private readonly BasketWebApplicationFactory _basketWebfactory;

    public IntegrationEventsScenarios(OrderingWebApplicationFactory orderingWebFactory, BasketWebApplicationFactory basketWebfactory)
    {
        _orderingWebFactory = orderingWebFactory;
        _basketWebfactory = basketWebfactory;
    }

    [Fact]
    public async Task Cancel_basket_and_check_order_status_cancelled()
    {
        // Expected data
        var cityExpected = $"city-{Guid.NewGuid()}";
        var orderStatusExpected = "Cancelled";

        var orderClient = _orderingWebFactory.CreateIdempotentClient();
        var basketClient = _basketWebfactory.CreateIdempotentClient();

        // GIVEN a basket is created
        var contentBasket = new StringContent(BuildBasket(), Encoding.UTF8, "application/json");
        await basketClient.PostAsync(Post.CreateBasket, contentBasket);

        // AND basket checkout is sent
        await basketClient.PostAsync(Post.CheckoutOrder, new StringContent(BuildCheckout(cityExpected), Encoding.UTF8, "application/json"));

        // WHEN Order is created in Ordering.api
        var newOrder = await TryGetNewOrderCreated(cityExpected, orderClient);

        // AND Order is cancelled in Ordering.api
        await orderClient.PutAsync(Put.CancelOrder(newOrder.OrderNumber), null);

        // AND the basket is retrieved
        var basket = await TryGetBasket(basketClient);

        // AND the requested order is retrieved
        var order = await TryGetOrder(newOrder.OrderNumber, orderStatusExpected, orderClient);

        // THEN check basket and order status
        Assert.Empty(basket!.Items);
        Assert.Equal(orderStatusExpected, order?.OrderStatus);
    }

    private async Task<CustomerBasket?> TryGetBasket(HttpClient basketClient)
    {
        var counter = 0;
        CustomerBasket? basket = null;

        while (counter < 20)
        {
            var basketGetResponse = await basketClient.GetStringAsync(BasketWebApplicationFactory.Get.Basket);
            basket = JsonSerializer.Deserialize<CustomerBasket>(basketGetResponse, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            })!;

            if (!basket.Items.Any())
            {
                break;
            }

            counter++;
            await Task.Delay(100);
        }

        return basket;
    }

    private async Task<Order?> TryGetOrder(int orderNumber, string orderStatus, HttpClient orderClient)
    {
        var counter = 0;
        Order? order = null;

        while (counter < 20)
        {
            var ordersGetResponse = await orderClient.GetStringAsync(OrderingWebApplicationFactory.Get.Orders);
            var orders = JsonSerializer.Deserialize<List<Order>>(ordersGetResponse, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            })!;

            order = orders.Single(o => o.OrderNumber == orderNumber);

            if (order.OrderStatus == orderStatus)
            {
                break;
            }

            counter++;
            await Task.Delay(100);
        }

        return order;
    }

    private async Task<Order> TryGetNewOrderCreated(string city, HttpClient orderClient)
    {
        var counter = 0;
        Order? order = null;

        while (counter < 20)
        {
            //get the orders and verify that the new order has been created
            var ordersGetResponse = await orderClient.GetStringAsync(OrderingWebApplicationFactory.Get.Orders);
            var orders = JsonSerializer.Deserialize<List<Order>>(ordersGetResponse, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (orders?.Count > 0)
            {
                var lastOrder = orders.OrderByDescending(o => o.OrderDate).First();
                var id = lastOrder.OrderNumber;
                var orderDetails = await orderClient.GetStringAsync(OrderingWebApplicationFactory.Get.OrderBy(id));
                order = JsonSerializer.Deserialize<Order>(orderDetails, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                })!;

                if (IsOrderCreated(order, city))
                {
                    break;
                }
            }

            counter++;
            await Task.Delay(100);
        }

        return order!;
    }

    private bool IsOrderCreated(Order order, string city)
    {
        return order.Address.City == city;
    }

    private string BuildBasket()
    {
        CustomerBasket order = new(AutoAuthorizeMiddleware.IDENTITY_ID)
        {
            Items = new List<BasketItem>()
            {
                new BasketItem()
                {
                    ProductName = "ProductName",
                    ProductId = 1,
                    UnitPrice = 10,
                    Quantity = 1
                }
            }
        };
        return JsonSerializer.Serialize(order);
    }

    private string BuildCheckout(string cityExpected)
    {
        basket.Microsoft.eShopOnDapr.Services.Basket.API.Model.BasketCheckout checkoutBasket = new(
            "buyer@email.com",
            cityExpected,
            "street",
            "state",
            "coutry",
            "1111111111111",
            "CardHolderName",
            DateTime.Now.AddYears(1),
            "123");

        return JsonSerializer.Serialize(checkoutBasket);
    }
}