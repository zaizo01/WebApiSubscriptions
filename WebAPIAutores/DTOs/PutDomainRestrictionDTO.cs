﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPIAutores.DTOs
{
    public class PutDomainRestrictionDTO
    {
        [Required]
        public string Domain { get; set; }
    }
}