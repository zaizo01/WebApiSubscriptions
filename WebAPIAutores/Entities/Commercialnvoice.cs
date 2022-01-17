using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPIAutores.Entities
{
    public class Commercialnvoice
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public bool Paid { get; set; }
        public decimal Amount { get; set; }
        public DateTime DateOfIssue { get; set; }
        public DateTime PayDayLimit { get; set; }
    }
}
