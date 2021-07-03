using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eShopOnDapr.BlazorClient.Ordering
{
    public class OrderState
    {
        public int OrderNumber { get; set; }

        public string Status { get; set; }
    }
}
