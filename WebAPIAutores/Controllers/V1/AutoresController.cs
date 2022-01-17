using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPIAutores.DTOs;
using WebAPIAutores.Entities;
using WebAPIAutores.Filters;
using WebAPIAutores.Utilities;

namespace WebAPIAutores.Controllers.V1
{
    [Route("api/v1/autores")]
    //[Route("api/autores")]
    [HeaderPresentAttribute("x-version", "1")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
    [ApiController]
    public class AutoresController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;
        private readonly IAuthorizationService authorizationService;

        public AutoresController(ApplicationDbContext context, IMapper mapper, 
            IConfiguration configuration, IAuthorizationService authorizationService)
        {
            this.context = context;
            this.mapper = mapper;
            this.configuration = configuration;
            this.authorizationService = authorizationService;
        }

        [HttpGet(Name = "GetAllAuthorsv1")]
        [AllowAnonymous]
        [ServiceFilter(typeof(HATEOASAuthorFilterAttribute))]
        public async Task<ActionResult<List<AutorGetDTO>>> GetAll([FromQuery] PaginationDTO paginationDTO)
        {
            var queryable = context.Autores.AsQueryable();
            await HttpContext.InsertPaginationParameterInHeader(queryable);
            var autores = await queryable.OrderBy(autor => autor.Name).Paginate(paginationDTO).ToListAsync();
            return mapper.Map<List<AutorGetDTO>>(autores);
        }

        [HttpGet("{id:int}", Name = "GetAuthorByIdv1")]
        [AllowAnonymous]
        [ServiceFilter(typeof(HATEOASAuthorFilterAttribute))]
        public async Task<ActionResult<AutorDTOConLibros>> GetById(int id)
        {
            var autor = await context.Autores
                .Include(autorDB => autorDB.AutoresLibros)
                .ThenInclude(autorLibroDB => autorLibroDB.Libro)
                .FirstOrDefaultAsync(autorDB => autorDB.Id == id);
            if (autor == null) return NotFound();
            var authorDto = mapper.Map<AutorDTOConLibros>(autor);
            return authorDto;
        }

        

        [HttpGet("{name}", Name = "GetAuthorByNamev1")]
        public async Task<ActionResult<List<AutorGetDTO>>> GetByName(string name)
        {
            var autores = await context.Autores.Where(autorDB => autorDB.Name.Contains(name)).ToListAsync();
            return mapper.Map<List<AutorGetDTO>>(autores);
        }

        [HttpPost(Name = "PostAuthorsv1")]
        public async Task<ActionResult> Post([FromBody] AutorDTO autorDTO)
        {
            var authorExist = await context.Autores.AnyAsync(x => x.Name == autorDTO.Name);
            if (authorExist) return BadRequest($"El {autorDTO.Name} ya existe.");
            var autor = mapper.Map<Autor>(autorDTO);
            context.Add(autor);
            await context.SaveChangesAsync();
            var autorDto = mapper.Map<AutorGetDTO>(autor);
            return CreatedAtRoute("GetAutorv1", new { id = autor.Id }, autorDto);
        }

        [HttpPut("{id:int}", Name = "PutAuthorv1")]
        public async Task<ActionResult> Put(AutorPostDTO autorDto, int id)
        {
            var autorExiste = await context.Autores.AnyAsync(x => x.Id == id);
            if (!autorExiste) return NotFound();
            var autor = mapper.Map<Autor>(autorDto);
            autor.Id = id;
            context.Update(autor);
            await context.SaveChangesAsync();
            return NoContent();
        }
        /// <summary>
        /// Delete author
        /// </summary>
        /// <param name="id">Author id to delete</param>
        /// <returns></returns>
        [HttpDelete("{id:int}", Name = "DeleteAuthorv1")]
        public async Task<ActionResult> Delete(int id)
        {
            var authorExist = await context.Autores.AnyAsync(x => x.Id == id);
            if (!authorExist) return NotFound();
            context.Remove(new Autor() { Id = id });
            await context.SaveChangesAsync();
            return Ok();
        }
    }
}
