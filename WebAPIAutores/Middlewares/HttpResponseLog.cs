using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPIAutores.Middlewares
{
    public static class HttpResponseLogExtensions
    {
        public static IApplicationBuilder UseHttpResponseLog(this IApplicationBuilder app)
        {
            return app.UseMiddleware<HttpResponseLog>();
        }
    }
    public class HttpResponseLog
    {
        private readonly RequestDelegate next;
        private readonly ILogger<HttpResponseLog> logger;

        public HttpResponseLog(RequestDelegate next, ILogger<HttpResponseLog> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            using (var ms = new MemoryStream())
            {
                var originalResponseBody = context.Response.Body;
                context.Response.Body = ms;

                await next(context);

                ms.Seek(0, SeekOrigin.Begin);
                string response = new StreamReader(ms).ReadToEnd();
                ms.Seek(0, SeekOrigin.Begin);

                await ms.CopyToAsync(originalResponseBody);
                context.Response.Body = originalResponseBody;

                logger.LogInformation(response);
            }
        }
    }
}
