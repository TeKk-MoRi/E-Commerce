using ECommerce.BuildingBlocks.Application;
using IdentityAccess.Application.Authorization;
using IdentityAccess.Application.Authorization.DTOs;
using IdentityAccess.Application.Common.Interfaces;
using MediatR;

namespace IdentityAccess.Application.Usecases.Roles.UpdateRolePermissions;

public sealed record UpdateRolePermissionsCommand(
    string RoleName,
    IReadOnlyCollection<string> PermissionCodes)
    : IRequest<Result<RolePermissionsResponse>>;

public sealed class UpdateRolePermissionsCommandHandler(
    IRolePermissionWriteService rolePermissionWriteService)
    : IRequestHandler<UpdateRolePermissionsCommand, Result<RolePermissionsResponse>>
{
    public async Task<Result<RolePermissionsResponse>> Handle(
        UpdateRolePermissionsCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.RoleName))
        {
            return Result<RolePermissionsResponse>.ValidationFailure(
                "Role.NameRequired",
                "Role name is required.");
        }

        var normalizedPermissionCodes = request.PermissionCodes
            .Where(code => !string.IsNullOrWhiteSpace(code))
            .Select(code => code.Trim().ToLowerInvariant())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (normalizedPermissionCodes.Length == 0)
        {
            return Result<RolePermissionsResponse>.ValidationFailure(
                "Role.PermissionRequired",
                "At least one permission code is required.");
        }

        var normalizedRoleName = request.RoleName.Trim().ToLowerInvariant();

        if (normalizedRoleName == ApplicationRoles.Admin &&
            !normalizedPermissionCodes.Contains(ApplicationPermissions.IdentityRolesManage))
        {
            return Result<RolePermissionsResponse>.ValidationFailure(
                "Role.AdminPermissionRequired",
                "Admin role must keep identity.roles.manage permission.");
        }

        return await rolePermissionWriteService.ReplaceAsync(
            normalizedRoleName,
            normalizedPermissionCodes,
            cancellationToken);
    }
}