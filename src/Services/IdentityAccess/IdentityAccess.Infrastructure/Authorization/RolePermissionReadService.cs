using IdentityAccess.Application.Authorization.DTOs;
using IdentityAccess.Application.Common.Interfaces;
using IdentityAccess.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace IdentityAccess.Infrastructure.Authorization;

public sealed class RolePermissionReadService(
    IdentityAccessDbContext dbContext)
    : IRolePermissionReadService
{
    public async Task<RolePermissionsResponse> GetByRoleNameAsync(
        string roleName,
        CancellationToken cancellationToken = default)
    {
        var normalizedRoleName = roleName.Trim().ToLowerInvariant();

        var permissions = await dbContext.RolePermissions
            .AsNoTracking()
            .Where(rolePermission => rolePermission.RoleName == normalizedRoleName)
            .OrderBy(rolePermission => rolePermission.Permission.Code)
            .Select(rolePermission => new PermissionResponse(
                rolePermission.Permission.Code,
                rolePermission.Permission.Description))
            .ToListAsync(cancellationToken);

        return new RolePermissionsResponse(
            normalizedRoleName,
            permissions);
    }
}