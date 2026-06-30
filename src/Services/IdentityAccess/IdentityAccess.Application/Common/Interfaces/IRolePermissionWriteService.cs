using ECommerce.BuildingBlocks.Application;
using IdentityAccess.Application.Authorization.DTOs;

namespace IdentityAccess.Application.Common.Interfaces;

public interface IRolePermissionWriteService
{
    Task<Result<RolePermissionsResponse>> ReplaceAsync(
        string roleName,
        IReadOnlyCollection<string> permissionCodes,
        CancellationToken cancellationToken = default);
}