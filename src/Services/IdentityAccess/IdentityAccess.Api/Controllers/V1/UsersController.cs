using ECommerce.BuildingBlocks.Application;
using IdentityAccess.Application.Authorization;
using IdentityAccess.Application.Usecases.Users.AssignRoleToUser;
using IdentityAccess.Application.Usecases.Users.UnassignRoleFromUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityAccess.Api.Controllers.V1;

[ApiController]
[Route("api/v1/users")]
public sealed class UsersController(ISender sender) : BaseController(sender)
{
    [HttpPost("{userId}/roles/{roleName}")]
    [Authorize(Policy = ApplicationPermissions.IdentityUsersManage)]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> AssignRole(
        string userId,
        string roleName,
        CancellationToken cancellationToken)
    {
        var result = await Sender.Send(
            new AssignRoleToUserCommand(userId, roleName),
            cancellationToken);

        return OkResult(result);
    }

    [HttpDelete("{userId}/roles/{roleName}")]
    [Authorize(Policy = ApplicationPermissions.IdentityUsersManage)]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UnassignRole(
        string userId,
        string roleName,
        CancellationToken cancellationToken)
    {
        var result = await Sender.Send(
            new UnassignRoleFromUserCommand(userId, roleName),
            cancellationToken);

        return OkResult(result);
    }
}