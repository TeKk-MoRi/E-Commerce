using IdentityAccess.Application.Common.Interfaces;
using IdentityAccess.Infrastructure.Authentication;
using IdentityAccess.Infrastructure.Authorization;
using IdentityAccess.Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
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

        services.AddDbContext<IdentityAccessDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("IdentityAccessDb"));
        });
        services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddScoped<IPermissionReadService, PermissionReadService>();

        return services;
    }
}
