using ECommerce.BuildingBlocks.Application;
using IdentityAccess.Application.Authorization;
using IdentityAccess.Application.Authorization.DTOs;
using IdentityAccess.Application.Usecases.Permissions.GetPermissions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityAccess.Api.Controllers.V1;

[ApiController]
[Route("api/v1/permissions")]
public sealed class PermissionsController(ISender sender) : BaseController(sender)
{
    [HttpGet]
    [Authorize(Policy = ApplicationPermissions.IdentityRolesManage)]
    [ProducesResponseType(typeof(Result<IReadOnlyCollection<PermissionDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new GetPermissionsQuery(), cancellationToken);

        return OkResult(result);
    }
}