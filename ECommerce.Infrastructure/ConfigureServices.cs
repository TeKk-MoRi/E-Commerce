using ECommerce.Application.Common.Interfaces;
using ECommerce.Infrastructure.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ECommerce.Infrastructure
{
    public static class ConfigureServices
    {
        public static IServiceCollection RegisterInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient();

            // Add Keycloak configuration
            services.Configure<KeycloakConfig>(configuration.GetSection("Keycloak"));

            // Add Keycloak service with HttpClient
            services.AddHttpClient<IKeycloakService, KeycloakService>();

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            });
            return services;
        }
    }
}
