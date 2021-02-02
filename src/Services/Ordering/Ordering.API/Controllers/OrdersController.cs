using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Dapr.Actors;
using Dapr.Actors.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.eShopOnContainers.Services.Ordering.API.Actors;
using Microsoft.eShopOnContainers.Services.Ordering.API.Infrastructure.Services;
using Microsoft.eShopOnContainers.Services.Ordering.API.Model;

namespace Microsoft.eShopOnContainers.Services.Ordering.API.Controllers
{
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
        public async Task<IActionResult> CancelOrderAsync(int orderNumber, [FromHeader(Name = "x-requestid")] string requestId)
        {
            bool result = false;

            if (Guid.TryParse(requestId, out Guid guid) && guid != Guid.Empty)
            {
                var orderingProcessActor = await GetOrderingProcessActorAsync(orderNumber);
                result = await orderingProcessActor.Cancel();
            }

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
                result = await orderingProcessActor.Ship();
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
            try
            {
                var order = await _orderRepository.GetOrderByOrderNumberAsync(orderNumber);

                return Ok(OrderDto.FromOrder(order));
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<OrderSummaryDto>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<OrderSummaryDto>>> GetOrdersAsync()
        {
            var buyerId = _identityService.GetUserIdentity();
            var orders = await _orderRepository.GetOrdersFromBuyerAsync(buyerId);

            return Ok(orders.Select(OrderSummaryDto.FromOrderSummary));
        }

        [Route("cardtypes")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CardTypeDto>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<CardType>>> GetCardTypesAsync()
        {
            var cardTypes = await _orderRepository.GetCardTypesAsync();

            return Ok(cardTypes.Select(CardTypeDto.FromCardType));
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
}
