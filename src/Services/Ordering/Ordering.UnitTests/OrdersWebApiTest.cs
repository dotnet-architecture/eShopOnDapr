namespace Ordering.UnitTests;

public class OrdersWebApiTest
{
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<IIdentityService> _identityServiceMock;
    private readonly Mock<IOrderingProcessActor> _orderingProcessActorMock;
    private readonly Mock<IActorProxyFactory> _proxyFactoryMock;

    public OrdersWebApiTest()
    {
        _orderRepositoryMock = new();
        _identityServiceMock = new();
        _orderingProcessActorMock = new();
        _proxyFactoryMock = new();

        _proxyFactoryMock.Setup(x => x.CreateActorProxy<IOrderingProcessActor>(It.IsAny<ActorId>(), nameof(OrderingProcessActor), null))
            .Returns(_orderingProcessActorMock.Object);
    }

    [Fact]
    public async Task Cancel_order_with_requestId_success()
    {
        //Arrange
        var fakeDynamicResult = new Order();
        _orderRepositoryMock.Setup(x => x.GetOrderByOrderNumberAsync(It.Is<int>(n => n == 1)))
            .Returns(Task.FromResult(fakeDynamicResult ?? null));
        _orderingProcessActorMock.Setup(x => x.CancelAsync())
            .Returns(Task.FromResult(true));

        //Act
        var orderController = new OrdersController(_orderRepositoryMock.Object, _identityServiceMock.Object, _proxyFactoryMock.Object);
        var actionResult = await orderController.CancelOrderAsync(1) as OkResult;

        //Assert
        Assert.Equal((int)System.Net.HttpStatusCode.OK, actionResult?.StatusCode);
    }

    [Fact]
    public async Task Cancel_order_bad_request()
    {
        //Arrange
        var fakeDynamicResult = new Order();
        _orderRepositoryMock.Setup(x => x.GetOrderByOrderNumberAsync(It.Is<int>(n => n == 1)))
            .Returns(Task.FromResult(fakeDynamicResult ?? null));
        _orderingProcessActorMock.Setup(x => x.CancelAsync())
            .Returns(Task.FromResult(false));

        //Act
        var orderController = new OrdersController(_orderRepositoryMock.Object, _identityServiceMock.Object, _proxyFactoryMock.Object);
        var actionResult = await orderController.CancelOrderAsync(1) as BadRequestResult;

        //Assert
        Assert.Equal((int)System.Net.HttpStatusCode.BadRequest, actionResult?.StatusCode);
    }

    [Fact]
    public async Task Ship_order_with_requestId_success()
    {
        //Arrange
        var fakeDynamicResult = new Order();
        _orderRepositoryMock.Setup(x => x.GetOrderByOrderNumberAsync(It.Is<int>(n => n == 1)))
            .Returns(Task.FromResult(fakeDynamicResult ?? null));
        _orderingProcessActorMock.Setup(x => x.ShipAsync())
            .Returns(Task.FromResult(true));

        //Act
        var orderController = new OrdersController(_orderRepositoryMock.Object, _identityServiceMock.Object, _proxyFactoryMock.Object);
        var actionResult = await orderController.ShipOrderAsync(1, Guid.NewGuid().ToString()) as OkResult;

        //Assert
        Assert.Equal((int)System.Net.HttpStatusCode.OK, actionResult?.StatusCode);
    }

    [Fact]
    public async Task Ship_order_bad_request()
    {
        //Arrange
        var fakeDynamicResult = new Order();
        _orderRepositoryMock.Setup(x => x.GetOrderByOrderNumberAsync(It.Is<int>(n => n == 1)))
            .Returns(Task.FromResult(fakeDynamicResult ?? null));
        _orderingProcessActorMock.Setup(x => x.ShipAsync())
            .Returns(Task.FromResult(false));

        //Act
        var orderController = new OrdersController(_orderRepositoryMock.Object, _identityServiceMock.Object, _proxyFactoryMock.Object);
        var actionResult = await orderController.ShipOrderAsync(1, Guid.NewGuid().ToString()) as BadRequestResult;

        //Assert
        Assert.Equal((int)System.Net.HttpStatusCode.BadRequest, actionResult?.StatusCode);
    }

    [Fact]
    public async Task Get_orders_success()
    {
        //Arrange
        var fakeDynamicResult = Enumerable.Empty<OrderSummary>();

        _identityServiceMock.Setup(x => x.GetUserIdentity())
            .Returns(Guid.NewGuid().ToString());

        _orderRepositoryMock.Setup(x => x.GetOrdersFromBuyerAsync(Guid.NewGuid().ToString()))
            .Returns(Task.FromResult(fakeDynamicResult));

        //Act
        var orderController = new OrdersController(_orderRepositoryMock.Object, _identityServiceMock.Object, _proxyFactoryMock.Object);
        var actionResult = await orderController.GetOrdersAsync();

        //Assert
        Assert.Equal((int)System.Net.HttpStatusCode.OK, (actionResult.Result as OkObjectResult)?.StatusCode);
    }

    [Fact]
    public async Task Get_order_success()
    {
        //Arrange
        var fakeOrderNumber = 123;
        var fakeDynamicResult = new Order();

        _identityServiceMock.Setup(x => x.GetUserIdentity())
            .Returns(string.Empty);

        _orderRepositoryMock.Setup(x => x.GetOrderByOrderNumberAsync(It.IsAny<int>()))
            .Returns(Task.FromResult(fakeDynamicResult ?? null));

        //Act
        var orderController = new OrdersController(_orderRepositoryMock.Object, _identityServiceMock.Object, _proxyFactoryMock.Object);
        var actionResult = await orderController.GetOrderAsync(fakeOrderNumber) as OkObjectResult;

        //Assert
        Assert.Equal((int)System.Net.HttpStatusCode.OK, actionResult?.StatusCode);
    }
}