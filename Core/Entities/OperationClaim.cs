﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class OperationClaim
    {
        [Key]
        public int RecordId { get; set; }
        public string Name { get; set; }
    }
}
