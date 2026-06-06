using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;

namespace ECommerce.Presentation.Authentication;

public class KeycloakRoleClaimsTransformation : IClaimsTransformation
{
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (principal.Identity is not ClaimsIdentity identity)
            return Task.FromResult(principal);

        if (!identity.IsAuthenticated)
            return Task.FromResult(principal);

        AddRealmRoles(identity);
        AddResourceRoles(identity);

        return Task.FromResult(principal);
    }

    private static void AddRealmRoles(ClaimsIdentity identity)
    {
        var realmAccess = identity.FindFirst("realm_access")?.Value;

        if (string.IsNullOrWhiteSpace(realmAccess))
            return;

        using var document = JsonDocument.Parse(realmAccess);

        if (!document.RootElement.TryGetProperty("roles", out var roles))
            return;

        foreach (var role in roles.EnumerateArray())
        {
            var roleName = role.GetString();

            if (string.IsNullOrWhiteSpace(roleName))
                continue;

            if (!identity.HasClaim(ClaimTypes.Role, roleName))
                identity.AddClaim(new Claim(ClaimTypes.Role, roleName));
        }
    }

    private static void AddResourceRoles(ClaimsIdentity identity)
    {
        var resourceAccess = identity.FindFirst("resource_access")?.Value;

        if (string.IsNullOrWhiteSpace(resourceAccess))
            return;

        using var document = JsonDocument.Parse(resourceAccess);

        foreach (var client in document.RootElement.EnumerateObject())
        {
            if (!client.Value.TryGetProperty("roles", out var roles))
                continue;

            foreach (var role in roles.EnumerateArray())
            {
                var roleName = role.GetString();

                if (string.IsNullOrWhiteSpace(roleName))
                    continue;

                if (!identity.HasClaim(ClaimTypes.Role, roleName))
                    identity.AddClaim(new Claim(ClaimTypes.Role, roleName));
            }
        }
    }
}