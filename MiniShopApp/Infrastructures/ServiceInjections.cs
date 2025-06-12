using Microsoft.Extensions.DependencyInjection.Extensions;
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

            // Add any services below
            services.TryAddTransient<IProductService, ProductService>();
            services.TryAddTransient<IOrderService, OrderService>();
            return services;
        }
    }
}
