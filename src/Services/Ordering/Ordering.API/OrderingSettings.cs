namespace Microsoft.eShopOnContainers.Services.Ordering.API
{
    public class OrderingSettings
    {
        public string ConnectionString { get; set; }

        public int GracePeriodTime { get; set; }

        public bool SendConfirmationEmail { get; set; }
    }
}
