using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MiniShopApp.Data;
using MiniShopApp.Infrastructures.Services.Implements;
using MiniShopApp.Infrastructures.Services.Interfaces;

namespace MiniShopApp.Infrastructures
{
    public static class ServiceInjections
    {
        public static IServiceCollection AddInfraServices(this IServiceCollection services, IConfiguration configuration)
        {
            //Service Injection for DbContextFactory
            services.AddDbContextFactory<AppDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("MyServerConnection"));
            });

            // Add any services below
            services.TryAddTransient<IProductService, ProductService>();
            services.TryAddTransient<IOrderService, OrderService>();
            return services;
        }
    }
}
