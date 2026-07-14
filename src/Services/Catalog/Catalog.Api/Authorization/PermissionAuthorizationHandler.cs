using Microsoft.AspNetCore.Authorization;
using Catalog.Application.Common.Interfaces;

namespace Catalog.Api.Authorization;

public sealed class PermissionAuthorizationHandler(
    IIdentityAccessPermissionClient permissionClient,
    ILogger<PermissionAuthorizationHandler> logger)
    : AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        if (context.User.Identity?.IsAuthenticated != true)
        {
            return;
        }

        try
        {
            var hasPermission =
                await permissionClient.HasPermissionAsync(
                    requirement.Permission);

            if (hasPermission)
            {
                context.Succeed(requirement);
            }
        }
        catch (HttpRequestException exception)
        {
            logger.LogError(
                exception,
                "IdentityAccess permission check failed for permission {Permission}.",
                requirement.Permission);
        }
        catch (TaskCanceledException exception)
        {
            logger.LogError(
                exception,
                "IdentityAccess permission check timed out for permission {Permission}.",
                requirement.Permission);
        }
    }
}