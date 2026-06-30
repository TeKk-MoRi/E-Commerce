using IdentityAccess.Application.Authorization.DTOs;

namespace IdentityAccess.Application.Common.Interfaces;

public interface IRolePermissionReadService
{
    Task<RolePermissionsResponse> GetByRoleNameAsync(
        string roleName,
        CancellationToken cancellationToken = default);
}