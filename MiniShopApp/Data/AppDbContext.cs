using Microsoft.EntityFrameworkCore;
using MiniShopApp.Models.Items;

namespace MiniShopApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Add any custom model configurations here
        }

        // Define DbSets for your entities here, e.g.:
        // public DbSet<Product> Products { get; set; }
    
        public DbSet<Product> Products { get; set; } = null!;
    }
}
