using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPIAutores.DTOs
{
    public class KeyPutDTO
    {
        public int KeyId { get; set; }
        public bool UpdateKey { get; set; }
        public bool IsActive { get; set; }
    }
}
