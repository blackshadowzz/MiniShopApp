using Domain.IdentityModel;
using Domain.IdentityModel.Schemas;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Options;
using MiniShopApp.Infrastructures.DataAccess.Configurations;
using MiniShopApp.Models.Customers;
using MiniShopApp.Models.Items;
using MiniShopApp.Models.Orders;
using MiniShopApp.Models.Settings;
using System.Reflection;

namespace MiniShopApp.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // Ensure Identity tables are configured
            builder.Entity<ApplicationUser>().ToTable("Users", SchemaNames.Security);
            builder.Entity<ApplicationRole>().ToTable("Roles", SchemaNames.Security);
            builder.Entity<ApplicationRoleClaim>().ToTable("RoleClaims", SchemaNames.Security);
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles", SchemaNames.Security);
            builder.Entity<ApplicationUserClaim>().ToTable("UserClaims", SchemaNames.Security);
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins", SchemaNames.Security);
            builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens", SchemaNames.Security);
            builder.ApplyConfiguration(new PermissionsConfigurations());
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        public DbSet<ApplicationRoleClaim> AppRoleClaims { get; set; }
        public DbSet<ApplicationUserClaim> AppUserClaims { get; set; }
        public DbSet<Permissions> Permissions => Set<Permissions>();
        // Define DbSets for your entities here, e.g.:

        public DbSet<Product> TbProducts { get; set; } = null!;
        public DbSet<TbTable> TbTables { get; set; } = null!;
        public DbSet<Category> TbCategories { get; set; } = null!;
        public DbSet<UserCustomer> TbUserCustomers { get; set; } = null!;
        public DbSet<CustomerType> TbCustomerTypes { get; set; } = null!;
        public DbSet<TbOrder> TbOrders { get; set; } = null!;
        public DbSet<TbOrderDetails> TbOrderDetails { get; set; } = null!;
        //Telegram store
        public DbSet<TbTelegramGroup> TbTelegramGroups { get; set; } = null!;
        public DbSet<TbTelegramBotToken> TbTelegramBotTokens { get; set; } = null!;
    }
}
