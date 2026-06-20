using IdentityAccess.Application.Common.Interfaces;
using IdentityAccess.Infrastructure.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityAccess.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection RegisterIdentityAccessInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<KeycloakConfig>(configuration.GetSection("Keycloak"));

        services.AddHttpClient<IKeycloakService, KeycloakService>();

        return services;
    }
}