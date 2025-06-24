using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MiniShopApp.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.json")
           .Build();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(config.GetConnectionString("MyConnection"))
                .Options;

            return new AppDbContext(options);

        }
    }
}
