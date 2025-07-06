
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.IdentityModel
{
    public class ApplicationUser : IdentityUser
    {

        public long? TelegramUserId { get; set; } // For telegram user
        public long? NameId { get; set; } //Name: Employee, Customer, etc.
        public int? EditSquence { get; set; } = 0;
        public string? RefreshToken { get; set; }
        public int? AprovedLevelId { get; set; }
        public bool? IsActive { get; set; } = true;
        public DateTime? RefreshTokenExpiryDate { get; set; }
        public DateTime? CreatedDT { get; set; } = DateTime.Now;
        public DateTime? ModifiedDT { get; set; } = DateTime.Now;

    }
    public class ApplicationUserClaim : IdentityUserClaim<string>
    {
        public string? Group { get; set; } = string.Empty;
    }
}
