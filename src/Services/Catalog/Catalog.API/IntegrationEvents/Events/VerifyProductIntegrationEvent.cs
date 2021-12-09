namespace Microsoft.eShopOnDapr.Services.Catalog.API.IntegrationEvents.Events;

public record VerifyProductIntegrationEvent(string BuyerId, int[] ProductIds) : IntegrationEvent;
