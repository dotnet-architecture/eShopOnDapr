using Microsoft.AspNetCore.Mvc;

namespace Microsoft.eShopOnDapr.Services.Basket.API.Controllers
{
    public class HomeController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            return new RedirectResult("~/swagger");
        }
    }
}
