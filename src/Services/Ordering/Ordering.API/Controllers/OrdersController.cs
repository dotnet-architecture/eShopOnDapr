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
    //[Authorize]
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

        [Route("test")]
        [HttpPost]
        public async Task<IActionResult> TestAsync()
        {
            var address = new Address
            {
                Street = "Goodeslaan 25",
                ZipCode = "1852 ER",
                City = "Heiloo",
                State = "NH",
                Country = "NL"
            };

            var order = new Order
            {
                BuyerId = "amolenk",
                BuyerName = "Alexander Molenkamp",
                Address = address,
                OrderDate = DateTime.UtcNow,
                OrderStatus = OrderStatus.Submitted,
                OrderItems = new List<OrderItem>
                {
                    new OrderItem
                    {
                        ProductId = 1,
                        ProductName = "Hoodie",
                        UnitPrice = 10,
                        Units = 1
                    }
                }
            };

            var result = await _orderRepository.GetOrAddOrderAsync(order);


            return Ok(result.Id);
        }


        [Route("{orderId:int}/cancel")]
        [HttpPut]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CancelOrderAsync(int orderId, [FromHeader(Name = "x-requestid")] string requestId)
        {
            bool result = false;

            if (Guid.TryParse(requestId, out Guid guid) && guid != Guid.Empty)
            {
                var orderingProcessActor = GetOrderingProcessActor(orderId);
                result = await orderingProcessActor.Cancel();
            }

            if (!result)
            {
                return BadRequest();
            }

            return Ok();
        }

        [Route("{orderId:int}/ship")]
        [HttpPut]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> ShipOrderAsync(int orderId, [FromHeader(Name = "x-requestid")] string requestId)
        {
            bool result = false;

            if (Guid.TryParse(requestId, out Guid guid) && guid != Guid.Empty)
            {
                var orderingProcessActor = GetOrderingProcessActor(orderId);
                result = await orderingProcessActor.Ship();
            }

            if (!result)
            {
                return BadRequest();
            }

            return Ok();
        }

        [Route("{orderId:int}")]
        [HttpGet]
        [ProducesResponseType(typeof(OrderDto),(int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> GetOrderAsync(int orderId)
        {
            try
            {
                var order = await _orderRepository.GetOrderAsync(orderId);

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
            var userid = _identityService.GetUserIdentity();
            var orders = await _orderRepository.GetOrdersFromBuyerAsync(userid);

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

        private static IOrderingProcessActor GetOrderingProcessActor(int orderId)
        {
            var actorId = new ActorId(orderId.ToString());
            return ActorProxy.Create<IOrderingProcessActor>(actorId, nameof(OrderingProcessActor));
        }
    }
}
