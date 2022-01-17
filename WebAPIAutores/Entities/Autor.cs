using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using WebAPIAutores.Validations;

namespace WebAPIAutores.Entities
{
    public class Autor
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength:120, ErrorMessage = "El campo {0} no debe de tener mas de {1} caracteres")]
        [FirstLetterUppercaseAttribute]
        public string Name { get; set; }
        public List<AutorLibro> AutoresLibros { get; set; }

    }
}
