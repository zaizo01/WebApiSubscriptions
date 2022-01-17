using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using WebAPIAutores.DTOs;

namespace WebAPIAutores.Controllers.V1
{
    [Route("api/v1")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RootController : ControllerBase
    {
        private readonly IAuthorizationService authorizationService;

        public RootController(IAuthorizationService authorizationService)
        {
            this.authorizationService = authorizationService;
        }

        [HttpGet(Name = "GetRoot")]
        public async Task<ActionResult<IEnumerable<DataHATEOAS>>> Get()
        {
            var dataHateoas = new List<DataHATEOAS>();

            var isAdmin = await authorizationService.AuthorizeAsync(User, "isAdmin");

            dataHateoas.Add(new DataHATEOAS(link: Url.Link("GetRoot", new { }), description: "self", method: "GET"));
            
            dataHateoas.Add(new DataHATEOAS(link: Url.Link("GetAllAuthors", new { }), description: "get all authors", method: "GET"));
            
            dataHateoas.Add(new DataHATEOAS(link: Url.Link("GetAuthorById", new { }), description: "get author by id", method: "GET"));
            
            dataHateoas.Add(new DataHATEOAS(link: Url.Link("GetAuthorByName", new { }), description: "get author by name", method: "GET"));
            
            dataHateoas.Add(new DataHATEOAS(link: Url.Link("GetAllBooks", new { }), description: "get all books", method: "GET"));

            dataHateoas.Add(new DataHATEOAS(link: Url.Link("GetBookById", new { }), description: "get book by id", method: "GET"));

            dataHateoas.Add(new DataHATEOAS(link: Url.Link("GetBookByTittle", new { }), description: "get author by tittle", method: "GET"));

            if (isAdmin.Succeeded)
            {

                dataHateoas.Add(new DataHATEOAS(link: Url.Link("PostAuthors", new { }), description: "create a author", method: "POST"));

                dataHateoas.Add(new DataHATEOAS(link: Url.Link("PostBook", new { }), description: "create a Book", method: "POST"));
            }

            return dataHateoas;
        }
    }
}
