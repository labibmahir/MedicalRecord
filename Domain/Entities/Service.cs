﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Service : BaseModel
    {
        [Key]
        public int Oid { get; set; }

        [Required]
        [StringLength(100)]
        public string ServiceName { get; set; }
    }
}