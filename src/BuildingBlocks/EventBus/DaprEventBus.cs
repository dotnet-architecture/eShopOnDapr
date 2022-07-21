namespace Microsoft.eShopOnDapr.BuildingBlocks.EventBus;

public class DaprEventBus : IEventBus
{
    private readonly DaprClient _dapr;
    private readonly DaprEventBusSettings _settings;
    private readonly ILogger _logger;

    public DaprEventBus(
        DaprClient dapr,
        IOptions<DaprEventBusSettings> options,
        ILogger<DaprEventBus> logger)
    {
        _dapr = dapr;
        _settings = options.Value;
        _logger = logger;
    }

    public async Task PublishAsync(IntegrationEvent integrationEvent)
    {
        var topicName = integrationEvent.GetType().Name;

        _logger.LogInformation(
            "Publishing event {@Event} to {PubsubName}.{TopicName}",
            integrationEvent,
            _settings.PubSubComponentName,
            topicName);

        // We need to make sure that we pass the concrete type to PublishEventAsync,
        // which can be accomplished by casting the event to dynamic. This ensures
        // that all event fields are properly serialized.
        await _dapr.PublishEventAsync(_settings.PubSubComponentName, topicName, (object)integrationEvent);
    }
}
