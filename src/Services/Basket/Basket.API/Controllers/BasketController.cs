namespace Microsoft.eShopOnDapr.Services.Basket.API.Controllers;

[Route("api/v1/[controller]")]
[Authorize(Policy = "ApiScope")]
[ApiController]
public class BasketController : ControllerBase
{
    private readonly IBasketRepository _repository;
    private readonly IIdentityService _identityService;
    private readonly IEventBus _eventBus;
    private readonly ILogger<BasketController> _logger;

    public BasketController(
        IBasketRepository repository,
        IIdentityService identityService,
        IEventBus eventBus,
        ILogger<BasketController> logger)
    {
        _repository = repository;
        _identityService = identityService;
        _eventBus = eventBus;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(CustomerBasket), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<CustomerBasket>> GetBasketAsync()
    {
        var userId = _identityService.GetUserIdentity();
        var basket = await _repository.GetBasketAsync(userId);

        return Ok(basket ?? new CustomerBasket(userId));
    }

    [HttpPost]
    [ProducesResponseType(typeof(CustomerBasket), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<CustomerBasket>> UpdateBasketAsync([FromBody] CustomerBasket value)
    {
        var userId = _identityService.GetUserIdentity();

        value.BuyerId = userId;

        return Ok(await _repository.UpdateBasketAsync(value));
    }

    [HttpPost("checkout")]
    [ProducesResponseType((int)HttpStatusCode.Accepted)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult> CheckoutAsync(
        [FromBody] BasketCheckout basketCheckout,
        [FromHeader(Name = "X-Request-Id")] string requestId)
    {
        var userId = _identityService.GetUserIdentity();

        var basket = await _repository.GetBasketAsync(userId);
        if (basket == null)
        {
            return BadRequest();
        }

        var eventRequestId = Guid.TryParse(requestId, out Guid parsedRequestId)
            ? parsedRequestId : Guid.NewGuid();

        var eventMessage = new UserCheckoutAcceptedIntegrationEvent(
            userId,
            basketCheckout.UserEmail,
            basketCheckout.City,
            basketCheckout.Street,
            basketCheckout.State,
            basketCheckout.Country,
            basketCheckout.CardNumber,
            basketCheckout.CardHolderName,
            basketCheckout.CardExpiration,
            basketCheckout.CardSecurityCode,
            eventRequestId,
            basket);

        // Once basket is checkout, sends an integration event to
        // ordering.api to convert basket to order and proceed with
        // order creation process
        await _eventBus.PublishAsync(eventMessage);

        return Accepted();
    }

    // DELETE api/values/5
    [HttpDelete]
    [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
    public async Task DeleteBasketAsync()
    {
        var userId = _identityService.GetUserIdentity();

        _logger.LogInformation("Deleting basket for user {UserId}...", userId);

        await _repository.DeleteBasketAsync(userId);
    }
}
