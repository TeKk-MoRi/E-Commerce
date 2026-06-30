using ECommerce.BuildingBlocks.Application;
using IdentityAccess.Api.Contracts.Roles;
using IdentityAccess.Application.Authorization;
using IdentityAccess.Application.Authorization.DTOs;
using IdentityAccess.Application.Usecases.Roles.GetRolePermissions;
using IdentityAccess.Application.Usecases.Roles.UpdateRolePermissions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityAccess.Api.Controllers.V1;

[ApiController]
[Route("api/v1/roles")]
public sealed class RolesController(ISender sender) : BaseController(sender)
{
    [HttpGet("{roleName}/permissions")]
    [Authorize(Policy = ApplicationPermissions.IdentityRolesManage)]
    [ProducesResponseType(typeof(Result<RolePermissionsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetPermissions(
        string roleName,
        CancellationToken cancellationToken)
    {
        var result = await Sender.Send(
            new GetRolePermissionsQuery(roleName),
            cancellationToken);

        return OkResult(result);
    }
    
    
    
    [HttpPut("{roleName}/permissions")]
    [Authorize(Policy = ApplicationPermissions.IdentityRolesManage)]
    [ProducesResponseType(typeof(Result<RolePermissionsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdatePermissions(
        string roleName,
        [FromBody] UpdateRolePermissionsRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Sender.Send(
            new UpdateRolePermissionsCommand(
                roleName,
                request.PermissionCodes),
            cancellationToken);

        return OkResult(result);
    }
}

