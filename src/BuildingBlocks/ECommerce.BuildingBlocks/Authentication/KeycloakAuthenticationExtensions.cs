using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace ECommerce.BuildingBlocks.Authentication;

public static class KeycloakAuthenticationExtensions
{
    public static IServiceCollection AddKeycloakJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = "Keycloak")
    {
        var keycloakOptions = configuration
            .GetSection(sectionName)
            .Get<KeycloakAuthenticationOptions>()
            ?? throw new InvalidOperationException($"{sectionName} configuration is missing.");

        if (string.IsNullOrWhiteSpace(keycloakOptions.Authority))
            throw new InvalidOperationException("Keycloak Authority configuration is missing.");

        if (string.IsNullOrWhiteSpace(keycloakOptions.Realm))
            throw new InvalidOperationException("Keycloak Realm configuration is missing.");

        if (string.IsNullOrWhiteSpace(keycloakOptions.ClientId))
            throw new InvalidOperationException("Keycloak ClientId configuration is missing.");

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = $"{keycloakOptions.Authority}/realms/{keycloakOptions.Realm}";
                options.Audience = keycloakOptions.ClientId;
                options.RequireHttpsMetadata = keycloakOptions.RequireHttpsMetadata;
                options.MapInboundClaims = false;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    NameClaimType = "preferred_username",
                    RoleClaimType = ClaimTypes.Role
                };
            });

        return services;
    }
}