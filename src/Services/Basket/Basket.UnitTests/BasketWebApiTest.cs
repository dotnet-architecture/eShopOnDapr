namespace Basket.UnitTests;

public class BasketWebApiTest
{
    private readonly Mock<IBasketRepository> _basketRepositoryMock;
    private readonly Mock<IBasketIdentityService> _identityServiceMock;
    private readonly Mock<IEventBus> _serviceBusMock;
    private readonly Mock<ILogger<BasketController>> _loggerMock;

    public BasketWebApiTest()
    {
        _basketRepositoryMock = new Mock<IBasketRepository>();
        _identityServiceMock = new Mock<IBasketIdentityService>();
        _serviceBusMock = new Mock<IEventBus>();
        _loggerMock = new Mock<ILogger<BasketController>>();
    }

    [Fact]
    public async Task Get_customer_basket_success()
    {
        //Arrange
        var fakeCustomerId = "1";
        var fakeCustomerBasket = GetCustomerBasketFake(fakeCustomerId);

        _basketRepositoryMock.Setup(x => x.GetBasketAsync(It.IsAny<string>()))
            .Returns(Task.FromResult(fakeCustomerBasket));
        _identityServiceMock.Setup(x => x.GetUserIdentity()).Returns(fakeCustomerId);

        _serviceBusMock.Setup(x => x.PublishAsync(It.IsAny<UserCheckoutAcceptedIntegrationEvent>()));

        //Act
        var basketController = new BasketController(
            _basketRepositoryMock.Object,
            _identityServiceMock.Object,
            _serviceBusMock.Object,
            _loggerMock.Object);

        var actionResult = await basketController.GetBasketAsync();
        var result = actionResult.Result as OkObjectResult;

        //Assert
        Assert.Equal(result?.StatusCode, (int)System.Net.HttpStatusCode.OK);
        var value = result!.Value as CustomerBasket;
        Assert.Equal(value?.BuyerId, fakeCustomerId);
    }

    [Fact]
    public async Task Post_customer_basket_success()
    {
        //Arrange
        var fakeCustomerId = "1";
        var fakeCustomerBasket = GetCustomerBasketFake(fakeCustomerId);

        _basketRepositoryMock.Setup(x => x.UpdateBasketAsync(It.IsAny<CustomerBasket>()))
            .Returns(Task.FromResult(fakeCustomerBasket));
        _identityServiceMock.Setup(x => x.GetUserIdentity()).Returns(fakeCustomerId);
        _serviceBusMock.Setup(x => x.PublishAsync(It.IsAny<UserCheckoutAcceptedIntegrationEvent>()));

        //Act
        var basketController = new BasketController(
            _basketRepositoryMock.Object,
            _identityServiceMock.Object,
            _serviceBusMock.Object,
            _loggerMock.Object);

        var actionResult = await basketController.UpdateBasketAsync(fakeCustomerBasket);
        var result = actionResult.Result as OkObjectResult;

        //Assert
        Assert.Equal(result?.StatusCode, (int)System.Net.HttpStatusCode.OK);
        var value = result!.Value as CustomerBasket;
        Assert.Equal(value?.BuyerId, fakeCustomerId);
    }

    [Fact]
    public async Task Doing_Checkout_Without_Basket_Should_Return_Bad_Request()
    {
        var fakeCustomerId = "2";
        _basketRepositoryMock.Setup(x => x.GetBasketAsync(It.IsAny<string>()))
            .Returns(Task.FromResult((CustomerBasket)null!));
        _identityServiceMock.Setup(x => x.GetUserIdentity()).Returns(fakeCustomerId);

        //Act
        var basketController = new BasketController(
            _basketRepositoryMock.Object,
            _identityServiceMock.Object,
            _serviceBusMock.Object,
            _loggerMock.Object);

        var result = await basketController.CheckoutAsync(new BasketCheckout(null!, null!, null!, null!, null!, null!, null!, default, null!), Guid.NewGuid().ToString()) as BadRequestResult;
        Assert.NotNull(result);
    }

    [Fact]
    public async Task Doing_Checkout_Wit_Basket_Should_Publish_UserCheckoutAccepted_Integration_Event()
    {
        var fakeCustomerId = "1";
        var fakeCustomerBasket = GetCustomerBasketFake(fakeCustomerId);

        _basketRepositoryMock.Setup(x => x.GetBasketAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(fakeCustomerBasket));

        _identityServiceMock.Setup(x => x.GetUserIdentity()).Returns(fakeCustomerId);

        var basketController = new BasketController(
            _basketRepositoryMock.Object,
            _identityServiceMock.Object,
            _serviceBusMock.Object,
            _loggerMock.Object)
        {
            ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = new ClaimsPrincipal(
                    new ClaimsIdentity(new Claim[] {
                        new Claim("sub", "testuser"),
                        new Claim("scope", "basket"),
                        new Claim(ClaimTypes.Name, "testuser")
                            }))
                }
            }
        };

        //Act
        var result = await basketController.CheckoutAsync(new BasketCheckout(null!, null!, null!, null!, null!, null!, null!, default, null!), Guid.NewGuid().ToString()) as AcceptedResult;

        _serviceBusMock.Verify(mock => mock.PublishAsync(It.IsAny<UserCheckoutAcceptedIntegrationEvent>()), Times.Once);

        Assert.NotNull(result);
    }

    private static CustomerBasket GetCustomerBasketFake(string fakeCustomerId)
    {
        return new CustomerBasket(fakeCustomerId)
        {
            Items = new List<BasketItem>()
            {
                new BasketItem()
            }
        };
    }
}