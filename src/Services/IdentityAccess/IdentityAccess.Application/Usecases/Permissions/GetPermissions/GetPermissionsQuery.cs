using ECommerce.BuildingBlocks.Application;
using IdentityAccess.Application.Authorization;
using IdentityAccess.Application.Common.Interfaces;
using MediatR;

namespace IdentityAccess.Application.Usecases.Permissions.GetPermissions;

public sealed record GetPermissionsQuery
    : IRequest<Result<IReadOnlyCollection<PermissionDto>>>;

public sealed class GetPermissionsQueryHandler(
    IPermissionReadService permissionReadService)
    : IRequestHandler<GetPermissionsQuery, Result<IReadOnlyCollection<PermissionDto>>>
{
    public async Task<Result<IReadOnlyCollection<PermissionDto>>> Handle(
        GetPermissionsQuery request,
        CancellationToken cancellationToken)
    {
        var permissions = await permissionReadService.GetAllAsync(cancellationToken);

        return Result<IReadOnlyCollection<PermissionDto>>.Success(permissions);
    }
}