namespace eShopOnDapr.BlazorClient.Ordering
{
    public record OrderState
    {
        public int OrderNumber { get; set; }

        public string Status { get; set; }
    }
}
