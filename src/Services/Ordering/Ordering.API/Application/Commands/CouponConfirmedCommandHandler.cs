using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.eShopOnContainers.Services.Ordering.API.Application.Commands;
using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.OrderAggregate;
using Microsoft.eShopOnContainers.Services.Ordering.Infrastructure.Idempotency;
using Microsoft.Extensions.Logging;

namespace Ordering.API.Application.Commands
{
    public class CouponConfirmedCommandHandler : IRequestHandler<CouponConfirmedCommand, bool>
    {
        private readonly IOrderRepository _orderRepository;

        public CouponConfirmedCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<bool> Handle(CouponConfirmedCommand command, CancellationToken cancellationToken)
        {
            var orderToUpdate = await _orderRepository.GetAsync(command.OrderNumber);

            if (orderToUpdate == null)
            {
                return false;
            }

            orderToUpdate.ProcessCouponConfirmed();

            return await _orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }

    public class CouponConfirmIdenfifiedCommandHandler : IdentifiedCommandHandler<CouponConfirmedCommand, bool>
    {
        public CouponConfirmIdenfifiedCommandHandler(
            IMediator mediator,
            IRequestManager requestManager,
            ILogger<IdentifiedCommandHandler<CouponConfirmedCommand, bool>> logger)
            : base(mediator, requestManager, logger)
        {
        }

        protected override bool CreateResultForDuplicateRequest()
        {
            return true;                // Ignore duplicate requests for processing order.
        }
    }
}
