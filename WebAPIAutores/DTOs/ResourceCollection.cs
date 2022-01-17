using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPIAutores.DTOs
{
    public class ResourceCollection<T>: Resource where T: Resource
    {
        public List<T> Values { get; set; }
    }
}
