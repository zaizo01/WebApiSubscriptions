using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPIAutores.DTOs
{
    public class LibroGetDTO
    {
        public int Id { get; set; }
        public string Tittle { get; set; }
        public DateTime FechaPublicacion { get; set; }
        public List<ComentarioGetDTO> Comentarios { get; set; }
    }
}
