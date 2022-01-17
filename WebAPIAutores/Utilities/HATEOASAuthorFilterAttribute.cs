using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPIAutores.DTOs;
using WebAPIAutores.Services;

namespace WebAPIAutores.Utilities
{
    public class HATEOASAuthorFilterAttribute: HATEOASFilterAttribute
    {
        private readonly LinkGenerator linkGenerator;

        public HATEOASAuthorFilterAttribute(LinkGenerator linkGenerator)
        {
            this.linkGenerator = linkGenerator;
        }

        public override async Task OnResultExecutionAsync(ResultExecutingContext context,
            ResultExecutionDelegate next)
        {
            var mustInclude = MustIncludeHATEOAS(context);

            if (!mustInclude)
            {
                await next();
                return;
            }

            var result = context.Result as ObjectResult;
            var authorDto = result.Value as AutorGetDTO;
            if (authorDto == null)
            {
                var authorsDto = result.Value as List<AutorGetDTO> ??
                    throw new ArgumentNullException("An instance was expected autorDTO or List<autorDTO>");

                authorsDto.ForEach(async author => await linkGenerator.GenerateLinks(author));
                result.Value = authorsDto;
            }
            else
            {
                await linkGenerator.GenerateLinks(authorDto);
            }
            //var model = result.Value as AutorGetDTO ?? throw new ArgumentNullException("An instance was expected autorDTO");
            //await linkGenerator.GenerateLinks(model);
            await next();
        }
    }
}
