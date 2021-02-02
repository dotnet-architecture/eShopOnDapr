using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.Services.Ordering.API.Model
{
    public interface IEmailService
    {
        Task SendOrderConfirmation(Order order);
    }
}
