using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.Services.Ordering.API.Application.Models
{
    public class CustomerBasket
    {
        public string BuyerId { get; set; }
        public List<BasketItem> Items { get; set; }
    }
}
