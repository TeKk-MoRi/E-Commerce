using IdentityAccess.Application.Authorization;
using IdentityAccess.Contracts.Authorization;
using IdentityAccess.Infrastructure.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityAccess.Api.Controllers.v1;

[ApiController]
[Authorize]
[Route("api/v1/authorization")]
public sealed class InternalAuthorizationController(
    IAuthorizationService authorizationService)
    : ControllerBase
{
    [HttpPost("check")]
    [ProducesResponseType(
        typeof(CheckPermissionResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CheckPermission(
        [FromBody] CheckPermissionRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Permission))
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Invalid permission",
                detail: "Permission is required.");
        }

        var permission = request.Permission
            .Trim()
            .ToLowerInvariant();

        var isRegisteredPermission = ApplicationPermissions.All.Contains(
            permission,
            StringComparer.Ordinal);

        if (!isRegisteredPermission)
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Invalid permission",
                detail: $"Permission '{permission}' is not registered.");
        }

        var authorizationResult =
            await authorizationService.AuthorizeAsync(
                User,
                resource: null,
                new PermissionRequirement(permission));

        return Ok(new CheckPermissionResponse(
            authorizationResult.Succeeded));
    }
}