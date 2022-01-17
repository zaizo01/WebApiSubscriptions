using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPIAutores.Utilities
{
    public static class HttpContextExtensions
    {
        public async static Task InsertPaginationParameterInHeader<T>(this HttpContext httpContext,
            IQueryable<T> queryable)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            double quantity = await queryable.CountAsync();
            httpContext.Response.Headers.Add("TotalNumberOfRecords", quantity.ToString());
        }
    }
}
