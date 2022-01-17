using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPIAutores.Entities
{
    public class Request
    {
        public int Id { get; set; }
        public int KeyId { get; set; }
        public DateTime RequestDate { get; set; }
        public KeyAPI Key { get; set; }
    }
}
