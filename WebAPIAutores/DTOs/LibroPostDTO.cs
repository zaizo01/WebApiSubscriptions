using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WebAPIAutores.Validations;

namespace WebAPIAutores.DTOs
{
    public class LibroPostDTO
    {
        [FirstLetterUppercase]
        [StringLength(maximumLength: 250)]
        public string Tittle { get; set; }
        public DateTime? FechaPublicacion { get; set; }
        public List<int> AutoresIds { get; set; }
    }
}
