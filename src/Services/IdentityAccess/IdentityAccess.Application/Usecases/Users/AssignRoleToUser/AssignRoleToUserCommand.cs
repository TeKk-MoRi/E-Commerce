using ECommerce.BuildingBlocks.Application;
using IdentityAccess.Application.Authorization;
using IdentityAccess.Application.Common.Interfaces;
using MediatR;

namespace IdentityAccess.Application.Usecases.Users.AssignRoleToUser;

public sealed record AssignRoleToUserCommand(
    string UserId,
    string RoleName)
    : IRequest<Result<bool>>;

public sealed class AssignRoleToUserCommandHandler(
    IKeycloakService keycloakService)
    : IRequestHandler<AssignRoleToUserCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        AssignRoleToUserCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.UserId))
        {
            return Result<bool>.ValidationFailure(
                "User.IdRequired",
                "User id is required.");
        }

        if (string.IsNullOrWhiteSpace(request.RoleName))
        {
            return Result<bool>.ValidationFailure(
                "Role.NameRequired",
                "Role name is required.");
        }

        var normalizedRoleName = request.RoleName.Trim().ToLowerInvariant();

        if (!ApplicationRoles.All.Contains(normalizedRoleName))
        {
            return Result<bool>.ValidationFailure(
                "Role.Invalid",
                $"Role '{normalizedRoleName}' is not supported.");
        }

        return await keycloakService.AssignRealmRoleToUserAsync(
            request.UserId.Trim(),
            normalizedRoleName,
            cancellationToken);
    }
}