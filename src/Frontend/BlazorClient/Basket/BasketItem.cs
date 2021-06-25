using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace eShopOnDapr.BlazorClient.Basket
{
    public class BasketItem
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
