using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Extensions;
using Microsoft.eShopOnContainers.Services.Ordering.API;
using Microsoft.eShopOnContainers.Services.Ordering.API.Application.Commands;
using Microsoft.Extensions.Logging;
using Ordering.API.Application.IntegrationEvents.Events;
using Serilog.Context;

namespace Ordering.API.Application.IntegrationEvents.EventHandling
{
    public class UserCheckoutAcceptedIntegrationEventHandler : IIntegrationEventHandler<UserCheckoutAcceptedIntegrationEvent>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UserCheckoutAcceptedIntegrationEventHandler> _logger;

        public UserCheckoutAcceptedIntegrationEventHandler(
            IMediator mediator,
            ILogger<UserCheckoutAcceptedIntegrationEventHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Integration event handler which starts the create order process
        /// </summary>
        /// <param name="@event">
        /// Integration event message which is sent by the
        /// basket.api once it has successfully process the 
        /// order items.
        /// </param>
        /// <returns></returns>
        public async Task Handle(UserCheckoutAcceptedIntegrationEvent integrationEvent)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{integrationEvent.Id}-{Program.AppName}"))
            {
                _logger.LogInformation("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", integrationEvent.Id, Program.AppName, integrationEvent);

                var result = false;

                if (integrationEvent.RequestId != Guid.Empty)
                {
                    using (LogContext.PushProperty("IdentifiedCommandId", integrationEvent.RequestId))
                    {
                        var createOrderCommand = new CreateOrderCommand(integrationEvent.Basket.Items,
                            integrationEvent.UserId, integrationEvent.UserName, integrationEvent.City,
                            integrationEvent.Street, integrationEvent.State, integrationEvent.Country,
                            integrationEvent.ZipCode, integrationEvent.CardNumber, integrationEvent.CardHolderName,
                            integrationEvent.CardExpiration, integrationEvent.CardSecurityNumber,
                            integrationEvent.CardTypeId, integrationEvent.CodeDiscount, integrationEvent.Discount);

                        var requestCreateOrder = new IdentifiedCommand<CreateOrderCommand, bool>(createOrderCommand, integrationEvent.RequestId);

                        _logger.LogInformation(
                            "----- Sending command: {CommandName} - {IdProperty}: {CommandId} ({@Command})",
                            requestCreateOrder.GetGenericTypeName(),
                            nameof(requestCreateOrder.Id),
                            requestCreateOrder.Id,
                            requestCreateOrder);

                        result = await _mediator.Send(requestCreateOrder);

                        if (result)
                        {
                            _logger.LogInformation("----- CreateOrderCommand suceeded - RequestId: {RequestId}", integrationEvent.RequestId);
                        }
                        else
                        {
                            _logger.LogWarning("CreateOrderCommand failed - RequestId: {RequestId}", integrationEvent.RequestId);
                        }
                    }
                }
                else
                {
                    _logger.LogWarning("Invalid IntegrationEvent - RequestId is missing - {@IntegrationEvent}", integrationEvent);
                }
            }
        }
    }
}