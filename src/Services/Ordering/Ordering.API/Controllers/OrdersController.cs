namespace Microsoft.eShopOnDapr.Services.Ordering.API.Controllers;

[Route("api/v1/[controller]")]
[Authorize]
[ApiController]
public class OrdersController : ControllerBase
{
    private readonly IOrderRepository _orderRepository;
    private readonly IIdentityService _identityService;

    public OrdersController(
        IOrderRepository orderRepository, 
        IIdentityService identityService)
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
    }

    [Route("{orderNumber:int}/cancel")]
    [HttpPut]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> CancelOrderAsync(int orderNumber)
    {
        var orderingProcessActor = await GetOrderingProcessActorAsync(orderNumber);

        var result = await orderingProcessActor.CancelAsync();
        if (!result)
        {
            return BadRequest();
        }

        return Ok();
    }

    [Route("{orderNumber:int}/ship")]
    [HttpPut]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> ShipOrderAsync(int orderNumber, [FromHeader(Name = "x-requestid")] string requestId)
    {
        bool result = false;

        if (Guid.TryParse(requestId, out Guid guid) && guid != Guid.Empty)
        {
            var orderingProcessActor = await GetOrderingProcessActorAsync(orderNumber);
            result = await orderingProcessActor.ShipAsync();
        }

        if (!result)
        {
            return BadRequest();
        }

        return Ok();
    }

    [Route("{orderNumber:int}")]
    [HttpGet]
    [ProducesResponseType(typeof(Model.Order),(int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult> GetOrderAsync(int orderNumber)
    {
        var buyerId = _identityService.GetUserIdentity();

        var order = await _orderRepository.GetOrderByOrderNumberAsync(orderNumber);

        if (order?.BuyerId == buyerId)
        {
            return Ok(order);
        }

        return NotFound();
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<OrderSummary>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<OrderSummary>>> GetOrdersAsync()
    {
        var buyerId = _identityService.GetUserIdentity();
        var orders = await _orderRepository.GetOrdersFromBuyerAsync(buyerId);

        return Ok(orders.OrderByDescending(o => o.OrderNumber));
    }

    private async Task<IOrderingProcessActor> GetOrderingProcessActorAsync(int orderNumber)
    {
        var order = await _orderRepository.GetOrderByOrderNumberAsync(orderNumber);
        if (order == null)
        {
            throw new ArgumentException($"Order with order number {orderNumber} not found.");
        }

        var actorId = new ActorId(order.Id.ToString());
        return ActorProxy.Create<IOrderingProcessActor>(actorId, nameof(OrderingProcessActor));
    }
}
