using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.IdentityModel
{
    public class SystemUser
    {
        public const string UserName = "Administrator";
        public const string Password = "Pa$$w0rd";
        public const string Email = "admins@miniapp.com";
        public const string DefaultRole = "SuperAdmin";

        public const string AdminUserName = "mango";
        public const string AdminPassword = "Pa$$w0rd";
        public const string AdminEmail = "mango@miniapp.com";
    }
    public class SystemRole
    {
        public const string SuperAdmin = nameof(SuperAdmin);
        public const string Admin = nameof(Admin);
    }
}
