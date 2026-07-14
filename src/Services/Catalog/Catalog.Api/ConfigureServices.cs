using Catalog.Api.Authorization;
using Catalog.Api.Services;
using Catalog.Application.Common.Interfaces;
using Catalog.Infrastructure.Authorization;
using Catalog.Contracts.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace Catalog.Api;

public static class ConfigureServices
{
    public static IServiceCollection RegisterPresentationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHttpContextAccessor();

        services.AddScoped<ICurrentUserService, CurrentUserService>();

        services.AddTransient<BearerTokenForwardingHandler>();

        var identityAccessBaseUrl =
            configuration["Services:IdentityAccess:BaseUrl"]
            ?? throw new InvalidOperationException(
                "Services:IdentityAccess:BaseUrl is not configured.");

        services
            .AddHttpClient<
                IIdentityAccessPermissionClient,
                IdentityAccessPermissionClient>(client =>
            {
                client.BaseAddress = new Uri(identityAccessBaseUrl);
                client.Timeout = TimeSpan.FromSeconds(5);
            })
            .AddHttpMessageHandler<BearerTokenForwardingHandler>();
        
        services.AddScoped<
            IAuthorizationHandler,
            PermissionAuthorizationHandler>();

        services.AddAuthorization(options =>
        {
            options.AddPolicy(
                CatalogPermissions.ProductsView,
                policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.AddRequirements(
                        new PermissionRequirement(
                            CatalogPermissions.ProductsView));
                });

            options.AddPolicy(
                CatalogPermissions.ProductsManage,
                policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.AddRequirements(
                        new PermissionRequirement(
                            CatalogPermissions.ProductsManage));
                });
        });

        return services;
    }
}