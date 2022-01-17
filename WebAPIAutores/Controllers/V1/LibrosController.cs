using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPIAutores.DTOs;
using WebAPIAutores.Entities;

namespace WebAPIAutores.Controllers.V1
{
    [Route("api/v1/libros")]
    [ApiController]
    public class LibrosController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public LibrosController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet(Name = "GetAllBooks")]
        public async Task<ActionResult<List<LibroGetDTO>>> GetAll()
        {
            var libros = await context.Libros.ToListAsync();
            return mapper.Map<List<LibroGetDTO>>(libros);
        }

        [HttpGet("{id:int}", Name = "GetBookById")]
        public async Task<ActionResult<LibroDTOConAutores>> GetById(int id)
        {
            var libro = await context.Libros
                .Include(libroDB => libroDB.Comentarios)
                .Include(librosDB => librosDB.AutoresLibros)
                .ThenInclude(autorLibroDB => autorLibroDB.Autor)
                .FirstOrDefaultAsync(libroDB => libroDB.Id == id);

            if (libro == null) return NotFound();

            libro.AutoresLibros = libro.AutoresLibros.OrderBy(x => x.Orden).ToList();

            return mapper.Map<LibroDTOConAutores>(libro);
        }

        [HttpGet("{tittle}", Name = "GetBookByTittle")]
        public async Task<ActionResult<List<LibroGetDTO>>> GetByName(string tittle)
        {
            var libros = await context.Libros.Where(libroDB => libroDB.Tittle.Contains(tittle)).ToArrayAsync();
            return mapper.Map<List<LibroGetDTO>>(libros);
        }

        [HttpPost(Name = "PostBook")]
        public async Task<ActionResult> Post(LibroPostDTO libroPostDTO)
        {

            if (libroPostDTO.AutoresIds == null) return BadRequest("No se puede crear un libro sin autores");

            var autoresIds = await context.Autores
                .Where(autorDB => libroPostDTO.AutoresIds.Contains(autorDB.Id)).Select(x => x.Id).ToListAsync();

            if (libroPostDTO.AutoresIds.Count != autoresIds.Count) return BadRequest("No existe uno de los autores enviados");

            var libro = mapper.Map<Libro>(libroPostDTO);
            AssignOrderAutores(libro);

            context.Add(libro);
            await context.SaveChangesAsync();
            var libroDTO = mapper.Map<LibroGetDTO>(libro);
            return CreatedAtRoute("GetBookById", new { id = libro.Id}, libroDTO);
        }

        [HttpPut("{id:int}", Name = "PutBook")]
        public async Task<ActionResult> Put(int id, LibroPostDTO libroPostDTO)
        {
            var libroDB = await context.Libros
                 .Include(x => x.AutoresLibros)
                 .FirstOrDefaultAsync(x => x.Id == id);

            if (libroDB == null) return NotFound();

            libroDB = mapper.Map(libroPostDTO, libroDB);
            AssignOrderAutores(libroDB);
            await context.SaveChangesAsync();
            return NoContent();
        }

        private void AssignOrderAutores(Libro libro)
        {
            if (libro.AutoresLibros != null)
            {
                for (int i = 0; i < libro.AutoresLibros.Count; i++)
                {
                    libro.AutoresLibros[i].Orden = i;
                }
            }
        }

        [HttpPatch("id:int", Name = "PatchBook")]
        public async Task<ActionResult> Patch (int id, JsonPatchDocument<LibroPatchDTO> patchDocument)
        {
            if (patchDocument == null) return BadRequest();

            var libroDB = await context.Libros.FirstOrDefaultAsync(x => x.Id == id);

            if (libroDB == null) return NotFound();

            var libroDTO = mapper.Map<LibroPatchDTO>(libroDB);
            patchDocument.ApplyTo(libroDTO, ModelState);

            var isValid = TryValidateModel(libroDTO);

            if (!isValid) return BadRequest(ModelState);

            mapper.Map(libroDTO, libroDB);

            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}", Name = "DeleteBook")]
        public async Task<ActionResult> Delete(int id)
        {
            var bookExist = await context.Libros.AnyAsync(x => x.Id == id);
            if (!bookExist) return NotFound();
            context.Remove(new Libro() { Id = id });
            await context.SaveChangesAsync();
            return Ok();
        }
    }
}
