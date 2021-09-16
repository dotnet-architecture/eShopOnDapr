using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;

namespace Microsoft.eShopOnDapr.Services.Identity.API
{
    public class CustomMessageStore<TModel> : ProtectedDataMessageStore<TModel>
    {
        private static readonly JsonSerializerOptions Options = new JsonSerializerOptions
        {
            IgnoreNullValues = true
        };

        public CustomMessageStore(IDataProtectionProvider provider, ILogger<ProtectedDataMessageStore<TModel>> logger)
            : base(provider, logger)
        {
        }

        public override Task<Message<TModel>> ReadAsync(string value)
        {
            Message<TModel> result = null;

            if (!string.IsNullOrWhiteSpace(value))
            {
                try
                {
                    var bytes = Base64Url.Decode(value);
                    bytes = Protector.Unprotect(bytes);
                    var json = Encoding.UTF8.GetString(bytes);
                    result = JsonSerializer.Deserialize<Message<TModel>>(value, Options);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Exception reading protected message");
                }
            }

            return Task.FromResult(result);
        }
    }
}
