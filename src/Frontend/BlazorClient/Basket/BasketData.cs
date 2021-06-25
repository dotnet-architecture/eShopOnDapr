using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace eShopOnDapr.BlazorClient.Basket
{
    public class BasketData
    {
        public string BuyerId { get; set; }

        public IEnumerable<BasketItem> Items { get; set; }
    }
}
