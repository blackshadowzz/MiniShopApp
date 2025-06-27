using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MiniShopApp.Data;
using MiniShopApp.Infrastructures.Services.Implements;
using MiniShopApp.Infrastructures.Services.Interfaces;
using MudBlazor;
using MudBlazor.Services;

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
            services.AddMudServices(config =>
            {
                config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopRight;

                config.SnackbarConfiguration.PreventDuplicates = false;
                config.SnackbarConfiguration.NewestOnTop = false;
                config.SnackbarConfiguration.ShowCloseIcon = true;
                config.SnackbarConfiguration.VisibleStateDuration = 4000;
                config.SnackbarConfiguration.HideTransitionDuration = 300;
                config.SnackbarConfiguration.ShowTransitionDuration = 500;
                config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
            });
            // Add any services below
            services.TryAddTransient<ITableListService, TableListService>();
            services.TryAddTransient<IProductService, ProductService>();
            services.AddScoped<IOrderService, OrderService>();
            services.TryAddTransient<IUserCustomerService, UserCustomerService>();
            services.TryAddTransient<ITelegramBotServices, TelegramBotServices>();
            services.TryAddTransient<ICustomerTypeService, CustomerTypeService>();
            return services;
        }
    }
}
