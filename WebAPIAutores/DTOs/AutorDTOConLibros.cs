using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPIAutores.DTOs
{
    public class AutorDTOConLibros: AutorGetDTO
    {
        public List<LibroGetDTO> Libros { get; set; }
    }
}
