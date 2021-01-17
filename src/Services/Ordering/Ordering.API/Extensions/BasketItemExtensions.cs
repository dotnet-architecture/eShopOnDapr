
using System;
using System.Collections.Generic;
using Microsoft.eShopOnContainers.Services.Ordering.API.Model;

namespace Ordering.API.Application.Models
{
    public static class BasketItemExtensions
    {
        public static IEnumerable<OrderItem> ToOrderItems(this IEnumerable<BasketItem> basketItems)
        {
            foreach (var item in basketItems)
            {
                yield return item.ToOrderItem();
            }
        }

        public static OrderItem ToOrderItem(this BasketItem item)
        {
            return new OrderItem()
            {
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                PictureUrl = item.PictureUrl,
                UnitPrice = item.UnitPrice,
                Units = item.Quantity
            };
        }
    }
}
