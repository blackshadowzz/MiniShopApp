using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.IdentityModel
{
    public class ApplicationRole :IdentityRole
    {
        public bool IsActive { get; set; } = true;
        public string? Description { get; set; } // Optional description for the role
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow; // Date when the role was created
        public DateTime ModifiedDate { get; set; } = DateTime.UtcNow; // Date when the role was last modified
    }
    
    public class ApplicationRoleClaim : IdentityRoleClaim<string>
    {
        public string? Module { get; set; } = string.Empty;
        public string? Group { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
    }
}
