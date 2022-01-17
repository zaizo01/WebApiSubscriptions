using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPIAutores.DTOs
{
    public class LibroDTOConAutores: LibroGetDTO
    {
        public List<AutorDTO> Autores { get; set; }
    }
}
