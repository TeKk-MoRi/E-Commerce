using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;

namespace ECommerce.Presentation.Authentication;

public sealed class KeycloakClaimsTransformation : IClaimsTransformation
{
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (principal.Identity is not ClaimsIdentity identity)
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

        if (!document.RootElement.TryGetProperty("roles", out var rolesElement))
            return;

        foreach (var role in rolesElement.EnumerateArray())
        {
            var roleName = role.GetString();

            if (string.IsNullOrWhiteSpace(roleName))
                continue;

            AddRoleIfMissing(identity, roleName);
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
            if (!client.Value.TryGetProperty("roles", out var rolesElement))
                continue;

            foreach (var role in rolesElement.EnumerateArray())
            {
                var roleName = role.GetString();

                if (string.IsNullOrWhiteSpace(roleName))
                    continue;

                AddRoleIfMissing(identity, roleName);
            }
        }
    }

    private static readonly HashSet<string> ApplicationRoles =
    [
        "admin",
        "customer"
    ];

    private static void AddRoleIfMissing(ClaimsIdentity identity, string role)
    {
        if (!ApplicationRoles.Contains(role))
            return;

        if (identity.HasClaim(ClaimTypes.Role, role))
            return;

        identity.AddClaim(new Claim(ClaimTypes.Role, role));
    }
}