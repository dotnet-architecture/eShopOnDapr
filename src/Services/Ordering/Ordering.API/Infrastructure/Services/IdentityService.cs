
using System;
using Microsoft.AspNetCore.Http;

namespace Microsoft.eShopOnDapr.Services.Ordering.API.Infrastructure.Services
{
    public class IdentityService : IIdentityService
    {
        private IHttpContextAccessor _context; 

        public IdentityService(IHttpContextAccessor context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public string GetUserIdentity()
            => _context.HttpContext.User.FindFirst("sub").Value;
    }
}
