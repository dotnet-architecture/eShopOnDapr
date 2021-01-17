using System;

namespace Microsoft.eShopOnContainers.Services.Ordering.API.Model
{
    public class OrderSummary
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public decimal Total { get; set; }
    }
}
