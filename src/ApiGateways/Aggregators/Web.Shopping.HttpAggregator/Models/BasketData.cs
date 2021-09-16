using System.Collections.Generic;

namespace Microsoft.eShopOnDapr.Web.Shopping.HttpAggregator.Models
{
    public class BasketData
    {
        public List<BasketDataItem> Items { get; set; } = new List<BasketDataItem>();

        public BasketData()
        {

        }
    }

    public class BasketDataItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public string PictureFileName { get; set; }
    }
}
