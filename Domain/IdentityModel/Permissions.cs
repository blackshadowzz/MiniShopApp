﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.IdentityModel
{
    public sealed class Permissions
    {
        public long Id { get; set; }
        public string? Module { get; set; }
        public string? Action { get; set; }
        public string? Group { get; set; }
        public string? Description { get; set; }
        public bool? IsBasic { get; set; } = false;
    }
}
