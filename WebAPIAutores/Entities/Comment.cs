using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPIAutores.Entities
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public int LibroId { get; set; }
        public Libro Libro { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
    }
}
