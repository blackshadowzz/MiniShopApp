using Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MiniShopApp.Data;
namespace Domain.IdentityModel
{
    public class DatabaseSeeders
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _context;

        public DatabaseSeeders(AppDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task DatabaseSeederAsync()
        {
            await CheckDatabaseMigrationAsync();
            await SeedDefaultRoles();
            await SeedSystemUser();
            await SeedAdminUser();
        }

        private async Task CheckDatabaseMigrationAsync()
        {
            if (_context.Database.GetPendingMigrations().Any())
            {
                await _context.Database.MigrateAsync();
            }
        }
        private async Task SeedDefaultRoles()
        {
            var defaultRoles = Enum.GetValues<AppRole>().Select(x => x.ToString());

            var dict = Enum.GetValues(typeof(AppRole)).Cast<AppRole>()
                .ToDictionary(t => t.ToString(), t => t.GetDisplayName());

            foreach (var roleName in defaultRoles)
            {
                if (await _roleManager.FindByNameAsync(roleName) is not ApplicationRole role)
                {
                    var appRole = new ApplicationRole
                    {
                        Name = roleName,
                        IsActive = true,
                        Description = dict[roleName].ToString(),
                    };
                    await _roleManager.CreateAsync(appRole);
                }
            }
        }
        private async Task SeedSystemUser()
        {
            var adminUser = new ApplicationUser
            {
                IsActive = true,
                UserName = SystemUser.UserName,
                Email = SystemUser.Email,
                EmailConfirmed = false,
                PhoneNumberConfirmed = false
            };
            if (!await _userManager.Users.AnyAsync(u => u.Email == adminUser.Email))
            {
                var password = new PasswordHasher<ApplicationUser>();

                adminUser.PasswordHash = password.HashPassword(adminUser, SystemUser.Password);

                await _userManager.CreateAsync(adminUser);

                if (!await _userManager.IsInRoleAsync(adminUser, SystemRole.SuperAdmin))
                {
                    await _userManager.AddToRoleAsync(adminUser, SystemRole.SuperAdmin);
                }
            }

        }
        private async Task SeedAdminUser()
        {
            var adminUser = new ApplicationUser
            {
                IsActive = true,
                UserName = SystemUser.AdminUserName,
                Email = SystemUser.AdminEmail,
                EmailConfirmed = false,
                PhoneNumberConfirmed = false
            };
            if (!await _userManager.Users.AnyAsync(u => u.Email == adminUser.Email))
            {
                var password = new PasswordHasher<ApplicationUser>();
                adminUser.PasswordHash = password.HashPassword(adminUser, SystemUser.Password);
                await _userManager.CreateAsync(adminUser);
                if (!await _userManager.IsInRoleAsync(adminUser, SystemRole.Admin))
                {
                    await _userManager.AddToRoleAsync(adminUser, SystemRole.Admin);
                }
            }
        }
    }

}
