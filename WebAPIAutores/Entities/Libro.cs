using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WebAPIAutores.Validations;

namespace WebAPIAutores.Entities
{
    public class Libro
    {
        public int Id { get; set; }
        [Required]
        [FirstLetterUppercase]
        [StringLength(maximumLength: 250)]
        public string Tittle { get; set; }
        public DateTime? FechaPublicacion { get; set; }
        public List<Comment> Comentarios { get; set; }
        public List<AutorLibro> AutoresLibros { get; set; }
    }
}
