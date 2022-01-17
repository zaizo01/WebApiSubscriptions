using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPIAutores.DTOs
{
    public class LimitRequestsConfiguration
    {
        public int FreeRequestForDay { get; set; }
        public string[] WhiteListRoutes { get; set; }
    }
}
