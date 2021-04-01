using System;
using System.Collections.Generic;

namespace eShopOnDapr.BlazorClient.Catalog
{
    public class CatalogPage
    {
        public int Count { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public int PageCount => (int)Math.Ceiling(Count / (decimal)PageSize);

        public IEnumerable<CatalogItem> Data { get; set; }
    }
}