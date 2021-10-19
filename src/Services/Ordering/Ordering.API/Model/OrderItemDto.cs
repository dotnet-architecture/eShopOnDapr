using System;

namespace Microsoft.eShopOnDapr.Services.Ordering.API.Model
{
    public class OrderItemDto
    {
        public string productname { get; set; }
        public int units { get; set; }
        public decimal unitprice { get; set; }
        public string pictureurl { get; set; }

        public static OrderItemDto FromOrderItem(OrderItem orderItem)
        {
            return new OrderItemDto
            {
                productname = orderItem.ProductName,
                units = orderItem.Units,
                unitprice = orderItem.UnitPrice,
                pictureurl = orderItem.PictureFileName
            };
        }
    }
}
