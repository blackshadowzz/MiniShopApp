using Microsoft.EntityFrameworkCore;
using MiniShopApp.Models;
using MiniShopApp.Models.Items;
using MiniShopApp.Models.Orders;

namespace MiniShopApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TbOrderDetails>()
                .HasOne<TbOrder>(s => s.TbOrder)
                .WithMany(g => g.TbOrderDetails)
                .HasForeignKey(s => s.OrderId);
            // Add any custom model configurations here
        }
        
       
        // Define DbSets for your entities here, e.g.:
        // public DbSet<Product> Products { get; set; }

        public DbSet<Product> TbProducts { get; set; } = null!;
        public DbSet<TbTable> TbTables { get; set; } = null!;
        public DbSet<Category> TbCategories { get; set; } = null!;
        public DbSet<UserCustomer> TbUserCustomers { get; set; } = null!;
        public DbSet<TbOrder> TbOrders { get; set; } = null!;
        public DbSet<TbOrderDetails> TbOrderDetails { get; set; } = null!;
    }
}
