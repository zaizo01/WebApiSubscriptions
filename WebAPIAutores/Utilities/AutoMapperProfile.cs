using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPIAutores.DTOs;
using WebAPIAutores.Entities;

namespace WebAPIAutores.Utilities
{
    public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<AutorDTO, Autor>();
            CreateMap<Autor, AutorPostDTO>().ReverseMap();
            CreateMap<Autor, AutorGetDTO>();
            CreateMap<Autor, AutorDTOConLibros>()
                .ForMember(autor => autor.Libros, options => options.MapFrom(MapAutorDTOLibros));
            CreateMap<LibroPostDTO, Libro>()
                .ForMember(libro => libro.AutoresLibros, options => options.MapFrom(MapAutoresLibros));
            CreateMap<Libro, LibroGetDTO>();
            CreateMap<Libro, LibroDTOConAutores>()
                .ForMember(libroDTO => libroDTO.Autores, options => options.MapFrom(MapLibroDTOAutores));
            CreateMap<ComentarioPostDTO, Comment>();
            CreateMap<Comment, ComentarioGetDTO>();
            CreateMap<LibroPatchDTO, Libro>().ReverseMap();
            CreateMap<KeyAPI, KeyDTO>();
            CreateMap<DomainRestriction, DomainRestrictionDTO>();
            CreateMap<IPRestriction, PostIpRestrictionDTO>();
        }

        private List<LibroGetDTO> MapAutorDTOLibros(Autor autor, AutorGetDTO autorGetDTO)
        {
            var result = new List<LibroGetDTO>();

            if (autor.AutoresLibros == null) return result;

            foreach (var autorLibro in autor.AutoresLibros)
            {
                result.Add(new LibroGetDTO()
                {
                    Id = autorLibro.LibroId,
                    Tittle = autorLibro.Libro.Tittle
                });
            }

            return result;
        }

        private List<AutorDTO> MapLibroDTOAutores(Libro libro, LibroGetDTO libroGetDTO)
        {
            var result = new List<AutorDTO>();

            if (libro.AutoresLibros == null) return result;

            foreach (var autorLibro in libro.AutoresLibros)
            {
                result.Add(new AutorDTO()
                {
                    Id = autorLibro.AutorId,
                    Name = autorLibro.Autor.Name
                });
            }

            return result;
        }

        private List<AutorLibro> MapAutoresLibros(LibroPostDTO libroPostDTO, Libro libro)
        {
            var result = new List<AutorLibro>();

            if (libroPostDTO.AutoresIds == null) return result;

            foreach (var autorId in libroPostDTO.AutoresIds)
            {
                result.Add(new AutorLibro() { AutorId = autorId });
            }

            return result;
        }
    }
}
