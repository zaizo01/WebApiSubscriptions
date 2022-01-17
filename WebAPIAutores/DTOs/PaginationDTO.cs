using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPIAutores.DTOs
{
    public class PaginationDTO
    {
        public int Page { get; set; } = 1;
        private int recordPerPage = 10;
        private readonly int maximunQuantityPerPages = 50;
        public int RecordPerPage 
        {
            get
            {
                return recordPerPage;
            }
            set
            {
                recordPerPage = (value > maximunQuantityPerPages) ? maximunQuantityPerPages : value;
            }
        }
    }
}
