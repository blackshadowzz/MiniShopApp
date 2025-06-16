using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Options;
using MiniShopApp.Models;
using MiniShopApp.Models.Items;
using MiniShopApp.Models.Orders;
using System.Reflection;

namespace MiniShopApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
         
          
    

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        }


        // Define DbSets for your entities here, e.g.:

        public DbSet<Product> TbProducts { get; set; } = null!;
        public DbSet<TbTable> TbTables { get; set; } = null!;
        public DbSet<Category> TbCategories { get; set; } = null!;
        public DbSet<UserCustomer> TbUserCustomers { get; set; } = null!;
        public DbSet<TbOrder> TbOrders { get; set; } = null!;
        public DbSet<TbOrderDetails> TbOrderDetails { get; set; } = null!;
    }
}
