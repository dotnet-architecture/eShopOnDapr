namespace Microsoft.eShopOnContainers.Services.Ordering.API
{
    public class OrderingSettings
    {
        public string ConnectionString { get; set; }

        // TODO Use
        public int GracePeriodTime { get; set; }

        public int CheckUpdateTime { get; set; }
    }
}
