using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using WebAPIAutores.DTOs;
using WebAPIAutores.Entities;
using WebAPIAutores.Utilities;

namespace WebAPIAutores.Controllers.V1
{
    [Route("api/v1/libros/{libroId:int}/comentarios")]
    [ApiController]
    public class ComentariosController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly UserManager<User> userManager;

        public ComentariosController(ApplicationDbContext context, IMapper mapper,
            UserManager<User> userManager)
        {
            this.context = context;
            this.mapper = mapper;
            this.userManager = userManager;
        }

        [HttpGet(Name = "GetAllComments")]
        public async Task<ActionResult<List<ComentarioGetDTO>>> Get(int libroId,
           [FromQuery] PaginationDTO paginationDTO)
        {
            var queryable = context.Comentarios
                .Where(comentarioDB => comentarioDB.LibroId == libroId).AsQueryable();
            await HttpContext.InsertPaginationParameterInHeader(queryable);
            var existBook = await context.Libros.AnyAsync(libroDB => libroDB.Id == libroId);
            if (!existBook) return NotFound();
            var comentarios = await queryable.OrderBy(comment => comment.Id).Paginate(paginationDTO).ToListAsync();
            return mapper.Map<List<ComentarioGetDTO>>(comentarios);
        }

        [HttpGet("{id:int}", Name = "GetCommentById")]
        public async Task<ActionResult<ComentarioGetDTO>> GetById(int id)
        {
            var comment = await context.Comentarios.FirstOrDefaultAsync(libro => libro.Id == id);
            if (comment == null) return NotFound();
            return mapper.Map<ComentarioGetDTO>(comment);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost(Name = "PostComment")]
        public async Task<ActionResult> Post(int libroId, ComentarioPostDTO comentarioPostDTO)
        {
            var emailUserClaim = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault();
            var userEmail = emailUserClaim.Value;
            var user = await userManager.FindByEmailAsync(userEmail);
            var userId = user.Id;
            
            var existBook = await context.Libros.AnyAsync(libroDB => libroDB.Id == libroId);
            if (!existBook) return NotFound();
            var comentario = mapper.Map<Comment>(comentarioPostDTO);
            comentario.LibroId = libroId;
            comentario.UserId = userId;
            context.Add(comentario);
            await context.SaveChangesAsync();
            var comentarioDTO = mapper.Map<ComentarioGetDTO>(comentario);
            return CreatedAtRoute("GetCommentById", new { id = comentario.Id, libroId = libroId}, comentarioDTO);
        }

        [HttpPut(Name = "PutComment")]
        public async Task<ActionResult> Put(int libroId, int id, ComentarioPostDTO comentarioDto)
        {
            var exiteLibro = await context.Libros.AnyAsync(libro => libro.Id == libroId);
            if (!exiteLibro) return NotFound();

            var existeComentario = await context.Comentarios.AnyAsync(comentario => comentario.Id == id);
            if (!existeComentario) return NotFound();

            var comentario = mapper.Map<Comment>(comentarioDto);
            comentario.Id = id;
            comentario.LibroId = libroId;
            context.Update(comentario);
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
