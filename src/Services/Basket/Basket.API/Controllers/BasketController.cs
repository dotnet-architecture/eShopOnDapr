using System;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Basket.API.IntegrationEvents.Events;
using Basket.API.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using Microsoft.eShopOnContainers.Services.Basket.API.Model;
using Microsoft.eShopOnContainers.Services.Basket.API.Services;
using Microsoft.Extensions.Logging;

namespace Microsoft.eShopOnContainers.Services.Basket.API.Controllers
{
    [Route("api/v1/[controller]")]
    [Authorize]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly IBasketRepository _repository;
        private readonly IIdentityService _identityService;
        private readonly IEventBus _eventBus;
        private readonly ILogger<BasketController> _logger;

        public BasketController(
            ILogger<BasketController> logger,
            IBasketRepository repository,
            IIdentityService identityService,
            IEventBus eventBus)
        {
            _logger = logger;
            _repository = repository;
            _identityService = identityService;
            _eventBus = eventBus;
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
        public async Task<ActionResult<CustomerBasket>> UpdateBasketAsync([FromBody]CustomerBasket value)
        {
            var userId = _identityService.GetUserIdentity();

            value.BuyerId = userId;

            return Ok(await _repository.UpdateBasketAsync(value));
        }

        [HttpPost("checkout")]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> CheckoutAsync(
            [FromBody]BasketCheckout basketCheckout,
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
                basketCheckout.CardSecurityNumber,
                eventRequestId,
                basket);

            // Once basket is checkout, sends an integration event to
            // ordering.api to convert basket to order and proceed with
            // order creation process
            try
            {
                await _eventBus.PublishAsync(eventMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR Publishing integration event: {IntegrationEventId} from {AppName}", eventMessage.Id, Program.AppName);

                throw;
            }

            return Accepted();
        }

        // DELETE api/values/5
        [HttpDelete]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        public async Task DeleteBasketByIdAsync(string id)
        {
            var userId = _identityService.GetUserIdentity();

            await _repository.DeleteBasketAsync(userId);
        }
    }
}
