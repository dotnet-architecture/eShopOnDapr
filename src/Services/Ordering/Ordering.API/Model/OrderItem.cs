using System;

namespace Microsoft.eShopOnDapr.Services.Ordering.API.Model
{
    public class OrderItem
    {
        public int Id { get; set; }
        public Guid OrderId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public int Units { get; set; }
        public string PictureFileName { get; set; }

        public static OrderItem FromActorState(Actors.OrderItem orderItem)
        {
            return new OrderItem
            {
                ProductId = orderItem.ProductId,
                ProductName = orderItem.ProductName,
                UnitPrice = orderItem.UnitPrice,
                Units = orderItem.Units,
                PictureFileName = orderItem.PictureFileName
            };
        }
    }
}