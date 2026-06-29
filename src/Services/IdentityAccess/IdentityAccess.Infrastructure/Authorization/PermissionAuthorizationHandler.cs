using IdentityAccess.Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace IdentityAccess.Infrastructure.Authorization;

public sealed class PermissionAuthorizationHandler(
    IdentityAccessDbContext dbContext)
    : AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        if (context.User.Identity is not { IsAuthenticated: true })
            return;

        var roles = context.User.Claims
            .Where(claim => claim.Type is ClaimTypes.Role or "role" or "roles")
            .Select(claim => claim.Value)
            .Where(role => !string.IsNullOrWhiteSpace(role))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (roles.Length == 0)
            return;

        var hasPermission = await dbContext.RolePermissions
            .AsNoTracking()
            .AnyAsync(rolePermission =>
                roles.Contains(rolePermission.RoleName) &&
                rolePermission.Permission.Code == requirement.Permission);

        if (hasPermission)
            context.Succeed(requirement);
    }
}