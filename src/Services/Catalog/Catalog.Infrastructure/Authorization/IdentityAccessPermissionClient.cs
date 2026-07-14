using System.Net;
using System.Net.Http.Json;
using Catalog.Application.Common.Interfaces;
using IdentityAccess.Contracts.Authorization;

namespace Catalog.Infrastructure.Authorization;

public sealed class IdentityAccessPermissionClient(
    HttpClient httpClient)
    : IIdentityAccessPermissionClient
{
    public async Task<bool> HasPermissionAsync(
        string permission,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(permission);

        using var response = await httpClient.PostAsJsonAsync(
            "api/v1/authorization/check",
            new CheckPermissionRequest(permission),
            cancellationToken);

        if (response.StatusCode is
            HttpStatusCode.Unauthorized or
            HttpStatusCode.Forbidden)
        {
            return false;
        }

        response.EnsureSuccessStatusCode();

        var result =
            await response.Content.ReadFromJsonAsync<CheckPermissionResponse>(
                cancellationToken);

        return result?.Allowed ?? false;
    }
}