using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPIAutores.DTOs;
using WebAPIAutores.Entities;

namespace WebAPIAutores.Middlewares
{
    public static class LimitRequestsMiddlewareExtensions
    {
        public static IApplicationBuilder UseLimitRequests(this IApplicationBuilder app)
        {
            return app.UseMiddleware<LimitRequestMiddleware>();
        }
    }

    public class LimitRequestMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IConfiguration configuration;

        public LimitRequestMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            this.next = next;
            this.configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext httpContext, ApplicationDbContext context)
        {
            var limitRequestConfiguration = new LimitRequestsConfiguration();
            configuration.GetSection("LimitRequest").Bind(limitRequestConfiguration);

            var route = httpContext.Request.Path.ToString();
            var isInWhiteList = limitRequestConfiguration.WhiteListRoutes.Any(x => route.Contains(x));

            if (isInWhiteList)
            {
                await next(httpContext);
                return;
            }

            var keyStringValues = httpContext.Request.Headers["X-Api-Key"];

            if (keyStringValues.Count == 0)
            {
                httpContext.Response.StatusCode = 400;
                await httpContext.Response.WriteAsync("You must provide the key in the header x-Api-Key");
                return;
            }

            if (keyStringValues.Count > 1)
            {
                httpContext.Response.StatusCode = 400;
                await httpContext.Response.WriteAsync("Only one key must be provide.");
                return;
            }

            var key = keyStringValues[0];

            var keyDB = await context.KeysAPI
                .Include(x => x.DomainRestriction)
                .Include(x => x.IPRestriction)
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Key == key);

            if(keyDB == null)
            {
                httpContext.Response.StatusCode = 400;
                await httpContext.Response.WriteAsync("The key no exist.");
                return;
            }

            if (!keyDB.IsActive)
            {
                httpContext.Response.StatusCode = 400;
                await httpContext.Response.WriteAsync("The key is not active.");
                return;
            }

            if (keyDB.KeyType == KeyType.Free)
            {
                var today = DateTime.Today;
                var tomorrow = today.AddDays(1);
                var quantityOfRequestPerToday = await context.Requests.CountAsync(x =>
                    x.KeyId == keyDB.Id && x.RequestDate >= today);

                if (quantityOfRequestPerToday >= limitRequestConfiguration.FreeRequestForDay)
                {
                    httpContext.Response.StatusCode = 429; // Too many request
                    await httpContext.Response.WriteAsync("Too many requests. Update your key for professional key!!");
                    return;
                }
            }
            else if (keyDB.User.PoorPay)
            {
                httpContext.Response.StatusCode = 400;
                await httpContext.Response.WriteAsync("The user is a bad pay.");
                return;
            }

            var overcomesRestriction = RequestOvercomesAnyRestriction(keyDB, httpContext);

            if (!overcomesRestriction)
            {
                httpContext.Response.StatusCode = 403;
                return;
            }

            var request = new Request() { KeyId = keyDB.Id, RequestDate = DateTime.UtcNow };
            context.Add(request);
            await context.SaveChangesAsync();

            await next(httpContext);
        }

        private bool RequestOvercomesAnyRestriction(KeyAPI keyAPI, HttpContext httpContext)
        {
            var thereAreRestrictions = keyAPI.DomainRestriction.Any() || keyAPI.IPRestriction.Any();

            if (!thereAreRestrictions)
            {
                return true;
            }

            var requestExceedDomainRestrictions = RequestExceedDomainRestrictions(keyAPI.DomainRestriction, httpContext);
            var requestExceedIPRestrictions = RequestExceedIPRestrictions(keyAPI.IPRestriction, httpContext);
            return requestExceedDomainRestrictions || requestExceedIPRestrictions;
        }

        private bool RequestExceedIPRestrictions(List<IPRestriction> iPRestrictions, HttpContext httpContext)
        {
            if (iPRestrictions == null || iPRestrictions.Count == 0) return false;

            var IP = httpContext.Connection.RemoteIpAddress.ToString();

            if (IP == string.Empty) return false;

            var overcomesRestriction = iPRestrictions.Any(x => x.IP == IP);
            return overcomesRestriction;
        }

        private bool RequestExceedDomainRestrictions(List<DomainRestriction> domainRestrictions, HttpContext httpContext)
        {
            if (domainRestrictions == null || domainRestrictions.Count == 0) return false;

            var referer = httpContext.Request.Headers["Referer"].ToString();

            if (referer == string.Empty) return false;

            Uri myUri = new Uri(referer);
            string host = myUri.Host;

            var overcomesRestriction = domainRestrictions.Any(x => x.Domain == host);
            return overcomesRestriction;
        }
    }
}
