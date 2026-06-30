using ECommerce.BuildingBlocks.Application;
using IdentityAccess.Application.Authorization;
using IdentityAccess.Application.Common.Interfaces;
using MediatR;

namespace IdentityAccess.Application.Usecases.Users.UnassignRoleFromUser;

public sealed record UnassignRoleFromUserCommand(
    string UserId,
    string RoleName)
    : IRequest<Result<bool>>;

public sealed class UnassignRoleFromUserCommandHandler(
    IKeycloakService keycloakService)
    : IRequestHandler<UnassignRoleFromUserCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        UnassignRoleFromUserCommand request,
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

        if (normalizedRoleName == ApplicationRoles.Admin)
        {
            return Result<bool>.ValidationFailure(
                "Role.AdminRemovalBlocked",
                "Admin role removal is blocked from this endpoint.");
        }

        return await keycloakService.UnassignRealmRoleFromUserAsync(
            request.UserId.Trim(),
            normalizedRoleName,
            cancellationToken);
    }
}