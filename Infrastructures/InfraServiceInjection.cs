using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructures
{
    public static class InfraServiceInjection
    {
        public static IServiceCollection AddInfraServices(this IServiceCollection services)
        {
            // Add your infrastructure services here
            // Example: services.Add

            services.TryAddTransient<IProductService, AppSettingService>();
            return services;
        }
    }
