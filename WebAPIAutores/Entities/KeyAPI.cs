using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPIAutores.Entities
{
    public class KeyAPI
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public KeyType KeyType { get; set; }
        public bool IsActive { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public List<DomainRestriction> DomainRestriction { get; set; }
        public List<IPRestriction> IPRestriction { get; set; }
    }

    public enum KeyType
    {
        Free = 1,
        Professional = 2
    }
}
