using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPIAutores.DTOs
{
    public class KeyDTO
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public bool IsActive { get; set; }
        public string KeyType { get; set; }

        public List<DomainRestrictionDTO> DomainRestriction { get; set; }
        public List<IpRestrictionDTO> IPRestriction { get; set; }

    }
}
