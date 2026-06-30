using ECommerce.BuildingBlocks.Application;
using IdentityAccess.Application.Authorization.DTOs;
using IdentityAccess.Application.Common.Interfaces;
using IdentityAccess.Domain.Authorization;
using IdentityAccess.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace IdentityAccess.Infrastructure.Authorization;

public sealed class RolePermissionWriteService(
    IdentityAccessDbContext dbContext)
    : IRolePermissionWriteService
{
    public async Task<Result<RolePermissionsResponse>> ReplaceAsync(
        string roleName,
        IReadOnlyCollection<string> permissionCodes,
        CancellationToken cancellationToken = default)
    {
        var normalizedRoleName = roleName.Trim().ToLowerInvariant();

        var normalizedPermissionCodes = permissionCodes
            .Where(code => !string.IsNullOrWhiteSpace(code))
            .Select(code => code.Trim().ToLowerInvariant())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var permissions = await dbContext.Permissions
            .Where(permission => normalizedPermissionCodes.Contains(permission.Code))
            .ToListAsync(cancellationToken);

        var existingPermissionCodes = permissions
            .Select(permission => permission.Code)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var invalidPermissionCodes = normalizedPermissionCodes
            .Where(code => !existingPermissionCodes.Contains(code))
            .ToArray();

        if (invalidPermissionCodes.Length > 0)
        {
            return Result<RolePermissionsResponse>.ValidationFailure(
                "invalidPermissionCodes.Length",$"Invalid permission code(s): {string.Join(", ", invalidPermissionCodes)}");
        }

        var existingRolePermissions = await dbContext.RolePermissions
            .Where(rolePermission => rolePermission.RoleName == normalizedRoleName)
            .ToListAsync(cancellationToken);

        dbContext.RolePermissions.RemoveRange(existingRolePermissions);

        var newRolePermissions = permissions
            .Select(permission => RolePermission.Create(
                normalizedRoleName,
                permission.Id))
            .ToArray();

        await dbContext.RolePermissions.AddRangeAsync(
            newRolePermissions,
            cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        var response = new RolePermissionsResponse(
            normalizedRoleName,
            permissions
                .OrderBy(permission => permission.Code)
                .Select(permission => new PermissionResponse(
                    permission.Code,
                    permission.Description))
                .ToArray());

        return Result<RolePermissionsResponse>.Success(response);
    }
}