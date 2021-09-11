using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Dapr.Actors;
using Dapr.Actors.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.eShopOnDapr.Services.Ordering.API.Actors;
using Microsoft.eShopOnDapr.Services.Ordering.API.Infrastructure;
using Microsoft.eShopOnDapr.Services.Ordering.API.Infrastructure.Services;
using Microsoft.eShopOnDapr.Services.Ordering.API.Model;

namespace Microsoft.eShopOnDapr.Services.Ordering.API.Controllers
{
    [Route("api/v1/[controller]")]
    [Authorize]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly OrderingDbContext _context;

        private readonly IOrderRepository _orderRepository;
        private readonly IIdentityService _identityService;

        public OrdersController(
            OrderingDbContext context,
            IOrderRepository orderRepository, 
            IIdentityService identityService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
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

            var order = await _context.Orders
                .Where(o => o.BuyerId == buyerId && o.OrderNumber == orderNumber)
                .Include(o => o.OrderItems)
                .SingleOrDefaultAsync();

            if (order != null)
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

            var orders = await _context.Orders
                .Where(o => o.BuyerId == buyerId)
                .Include(o => o.OrderItems)
                .Select(o => new OrderSummary
                {
                    Id = o.Id,
                    OrderNumber = o.OrderNumber,
                    OrderDate = o.OrderDate,
                    OrderStatus = o.OrderStatus,
                    Total = o.GetTotal()
                })
                .OrderByDescending(o => o.OrderNumber)
                .ToListAsync();

            return Ok(orders);
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
