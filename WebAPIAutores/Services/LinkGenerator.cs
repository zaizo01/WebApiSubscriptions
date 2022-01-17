using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPIAutores.DTOs;

namespace WebAPIAutores.Services
{
    public class LinkGenerator
    {
        private readonly IAuthorizationService authorizationService;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IActionContextAccessor actionContextAccessor;

        public LinkGenerator(IAuthorizationService authorizationService,
            IHttpContextAccessor httpContextAccessor,
            IActionContextAccessor actionContextAccessor)
        {
            this.authorizationService = authorizationService;
            this.httpContextAccessor = httpContextAccessor;
            this.actionContextAccessor = actionContextAccessor;
        }

        private IUrlHelper BuildURLHelper()
        {
            var factory = httpContextAccessor.HttpContext.RequestServices.GetRequiredService<IUrlHelperFactory>();
            return factory.GetUrlHelper(actionContextAccessor.ActionContext);
        }

        private async Task<bool> IsAdmin()
        {
            var httpContext = httpContextAccessor.HttpContext;
            var result = await authorizationService.AuthorizeAsync(httpContext.User, "IsAdmin");
            return result.Succeeded;
        }

        public async Task GenerateLinks(AutorGetDTO autorGetDTO)
        {
            var isAdmin = await IsAdmin();
            var Url = BuildURLHelper();

            autorGetDTO.Links.Add(new DataHATEOAS(
                link: Url.Link("GetAuthorById", new { id = autorGetDTO.Id }),
                description: "get author by id",
                method: "GET"));

            if (isAdmin)
            {
                autorGetDTO.Links.Add(new DataHATEOAS(
                    link: Url.Link("PutAuthor", new { id = autorGetDTO.Id }),
                    description: "put author",
                    method: "PUT"));

                autorGetDTO.Links.Add(new DataHATEOAS(
                    link: Url.Link("DeleteAuthor", new { id = autorGetDTO.Id }),
                    description: "delete author",
                    method: "DELETE"));
            }

        }
    }
}
