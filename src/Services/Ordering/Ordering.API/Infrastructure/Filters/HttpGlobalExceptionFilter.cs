namespace Microsoft.eShopOnContainers.Services.Ordering.API.Infrastructure.Filters
{
    using System.Net;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.eShopOnContainers.Services.Ordering.API.Infrastructure.ActionResults;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    public class HttpGlobalExceptionFilter : IExceptionFilter
    {
        private readonly IWebHostEnvironment env;
        private readonly ILogger<HttpGlobalExceptionFilter> logger;

        public HttpGlobalExceptionFilter(IWebHostEnvironment env, ILogger<HttpGlobalExceptionFilter> logger)
        {
            this.env = env;
            this.logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            logger.LogError(new EventId(context.Exception.HResult),
                context.Exception,
                context.Exception.Message);
            
            var json = new JsonErrorResponse
            {
                Messages = new[] { "An error occur.Try it again." }
            };

            if (env.IsDevelopment())
            {
                json.DeveloperMessage = context.Exception;
            }

            // Result asigned to a result object but in destiny the response is empty. This is a known bug of .net core 1.1
            // It will be fixed in .net core 1.1.2. See https://github.com/aspnet/Mvc/issues/5594 for more information
            context.Result = new InternalServerErrorObjectResult(json);
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            context.ExceptionHandled = true;
        }

        private class JsonErrorResponse
        {
            public string[] Messages { get; set; }

            public object DeveloperMessage { get; set; }
        }
    }
}
