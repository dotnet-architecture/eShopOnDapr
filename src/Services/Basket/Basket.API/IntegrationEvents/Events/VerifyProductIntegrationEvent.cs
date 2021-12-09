namespace Microsoft.eShopOnDapr.Services.Basket.API.IntegrationEvents.Events;

public record VerifyProductIntegrationEvent(string BuyerId, int[] ProductIds) : IntegrationEvent;
