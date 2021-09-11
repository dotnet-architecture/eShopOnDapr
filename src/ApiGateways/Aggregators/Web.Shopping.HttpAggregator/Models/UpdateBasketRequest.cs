using System.Collections.Generic;

namespace Microsoft.eShopOnDapr.Web.Shopping.HttpAggregator.Models
{
    public class UpdateBasketRequest
    {
        public IEnumerable<UpdateBasketRequestItemData> Items { get; set; }
    }

    public class UpdateBasketRequestItemData
    {
        public int ProductId { get; set; }      // Catalog item id
        public int Quantity { get; set; }       // Quantity
    }
}
