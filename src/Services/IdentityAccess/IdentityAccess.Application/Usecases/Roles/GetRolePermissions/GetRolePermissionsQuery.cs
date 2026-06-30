using ECommerce.BuildingBlocks.Application;
using IdentityAccess.Application.Authorization.DTOs;
using IdentityAccess.Application.Common.Interfaces;
using MediatR;

namespace IdentityAccess.Application.Usecases.Roles.GetRolePermissions;

public sealed record GetRolePermissionsQuery(string RoleName)
    : IRequest<Result<RolePermissionsResponse>>;

public sealed class GetRolePermissionsQueryHandler(
    IRolePermissionReadService rolePermissionReadService)
    : IRequestHandler<GetRolePermissionsQuery, Result<RolePermissionsResponse>>
{
    public async Task<Result<RolePermissionsResponse>> Handle(
        GetRolePermissionsQuery request,
        CancellationToken cancellationToken)
    {
        var rolePermissions = await rolePermissionReadService.GetByRoleNameAsync(
            request.RoleName,
            cancellationToken);

        return Result<RolePermissionsResponse>.Success(rolePermissions);
    }
}