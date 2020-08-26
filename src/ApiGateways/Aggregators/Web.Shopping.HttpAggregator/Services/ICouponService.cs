using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.Web.Shopping.HttpAggregator.Services
{
    public interface ICouponService
    {
        Task<HttpResponseMessage> CheckCouponByCodeNumberAsync(string codeNumber);
    }
}
