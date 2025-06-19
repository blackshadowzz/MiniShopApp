using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MiniShopApp.Data;
using MiniShopApp.Infrastructures.Services.Implements;
using MiniShopApp.Infrastructures.Services.Interfaces;
using Radzen;

namespace MiniShopApp.Infrastructures
{
    public static class ServiceInjections
    {
        public static IServiceCollection AddInfraServices(this IServiceCollection services, IConfiguration configuration)
        {
            //Service Injection for DbContextFactory
            services.AddDbContextFactory<AppDbContext>(options =>
            {
                options.ConfigureWarnings(w => w.Ignore(RelationalEventId.MigrationsUserTransactionWarning));

                options.UseSqlServer(configuration.GetConnectionString("MyConnection"));
            });
            // Additional services can be added here
            services.AddScoped<NotificationService>();
            services.AddScoped<DialogService>();
            services.AddScoped<TooltipService>();
            // Add any services below
            services.TryAddTransient<ITableListService, TableListService>();
            services.TryAddTransient<IProductService, ProductService>();
            services.TryAddTransient<IOrderService, OrderService>();
            services.TryAddTransient<IUserCustomerService, UserCustomerService>();
            return services;
        }
    }
}
