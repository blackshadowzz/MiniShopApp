using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.IdentityModel
{
    public sealed class UserProfile
    {
        public long Id { get; set; }
        public string UserRefId { get; set; } = string.Empty;
        public string? FileTitle { get; set; } = string.Empty;
        public string? FileName { get; set; } = string.Empty;
        public string? FileUrl { get; set; } = string.Empty;
        public DateTime? Created { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;
        public ApplicationUser? AppUser { get; set; }
    }
}
