using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Microsoft.eShopOnDapr.BlazorClient.Host.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SettingsController : Controller
    {
        [HttpGet]
        public IActionResult GetSettings([FromServices] IOptions<Settings> settings)
        {
            if (settings is null) throw new ArgumentNullException(nameof(settings));

            return Ok(settings.Value);
        }
    }
}
