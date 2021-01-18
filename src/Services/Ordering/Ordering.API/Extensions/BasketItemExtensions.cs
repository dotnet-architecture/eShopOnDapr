
//using System;
//using System.Collections.Generic;
//using Microsoft.eShopOnContainers.Services.Ordering.API.Model;

//namespace Ordering.API.Application.Models
//{
//    public static class BasketItemExtensions
//    {
//        public static IEnumerable<OrderItemState> ToOrderItems(this IEnumerable<BasketItem> basketItems)
//        {
//            foreach (var item in basketItems)
//            {
//                yield return item.ToOrderItem();
//            }
//        }

//        public static OrderItemState ToOrderItem(this BasketItem item)
//        {
//            return new OrderItemState()
//            {
//                ProductId = item.ProductId,
//                ProductName = item.ProductName,
//                PictureUrl = item.PictureUrl,
//                UnitPrice = item.UnitPrice,
//                Units = item.Quantity
//            };
//        }
//    }
//}
