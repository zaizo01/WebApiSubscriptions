using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPIAutores.Entities
{
    public class DomainRestriction
    {
        public int Id { get; set; }
        public int KeyId { get; set; }
        public string Domain { get; set; }
        public KeyAPI Key { get; set; }
    }
}
