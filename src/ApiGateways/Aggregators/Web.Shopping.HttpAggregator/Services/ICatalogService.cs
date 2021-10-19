using Microsoft.eShopOnDapr.Web.Shopping.HttpAggregator.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.eShopOnDapr.Web.Shopping.HttpAggregator.Services
{
    public interface ICatalogService
    {
        Task<IEnumerable<CatalogItem>> GetCatalogItemsAsync(IEnumerable<int> ids);
    }
}
