namespace Microsoft.eShopOnDapr.Services.Basket.API.IntegrationEvents.Events;

public record BasketValidatedIntegrationEvent(
    string BasketActorId,
    string Description,
    decimal Total,
    string BuyerId)
    : IntegrationEvent;