using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MiniShopApp.Data;
using MiniShopApp.Services.Implements;
using MiniShopApp.Services.Interfaces;

namespace MiniShopApp.Infrastructures
{
    public static class ServiceInjections
    {
        public static IServiceCollection AddInfraServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Add your infrastructure services here
            // Example: services.Add
            services.AddDbContextFactory<AppDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("MyConnection"));
            });
            // Add any services below
            services.TryAddTransient<IProductService, ProductService>();
            services.TryAddTransient<IOrderService, OrderService>();
            return services;
        }
    }
}
