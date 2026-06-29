using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Json;

namespace ECommerce.BuildingBlocks.Authentication;

public sealed class KeycloakRoleClaimsTransformation(
    IOptions<KeycloakAuthenticationOptions> options)
    : IClaimsTransformation
{
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (principal.Identity is not ClaimsIdentity identity || !identity.IsAuthenticated)
            return Task.FromResult(principal);

        AddRealmRoles(identity);
        AddClientRoles(identity, options.Value.ClientId);

        return Task.FromResult(principal);
    }

    private static void AddRealmRoles(ClaimsIdentity identity)
    {
        var realmAccessClaim = identity.FindFirst("realm_access");

        if (realmAccessClaim is null || string.IsNullOrWhiteSpace(realmAccessClaim.Value))
            return;

        using var document = JsonDocument.Parse(realmAccessClaim.Value);

        if (!document.RootElement.TryGetProperty("roles", out var rolesElement))
            return;

        AddRoles(identity, rolesElement);
    }

    private static void AddClientRoles(ClaimsIdentity identity, string clientId)
    {
        if (string.IsNullOrWhiteSpace(clientId))
            return;

        var resourceAccessClaim = identity.FindFirst("resource_access");

        if (resourceAccessClaim is null || string.IsNullOrWhiteSpace(resourceAccessClaim.Value))
            return;

        using var document = JsonDocument.Parse(resourceAccessClaim.Value);

        if (!document.RootElement.TryGetProperty(clientId, out var clientElement))
            return;

        if (!clientElement.TryGetProperty("roles", out var rolesElement))
            return;

        AddRoles(identity, rolesElement);
    }

    private static void AddRoles(ClaimsIdentity identity, JsonElement rolesElement)
    {
        if (rolesElement.ValueKind != JsonValueKind.Array)
            return;

        foreach (var roleElement in rolesElement.EnumerateArray())
        {
            var role = roleElement.GetString();

            if (string.IsNullOrWhiteSpace(role))
                continue;

            var alreadyExists = identity.HasClaim(ClaimTypes.Role, role);

            if (!alreadyExists)
                identity.AddClaim(new Claim(ClaimTypes.Role, role));
        }
    }
}