using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPIAutores.Entities
{
    public class IPRestriction
    {
        public int Id { get; set; }
        public int KeyId { get; set; }
        public string IP { get; set; }
        public KeyAPI Key { get; set; }
    }
}
