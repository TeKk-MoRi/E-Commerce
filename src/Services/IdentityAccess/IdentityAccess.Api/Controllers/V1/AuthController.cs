using System.Security.Claims;
using ECommerce.BuildingBlocks.Application;
using IdentityAccess.Api.Contracts.Auth;
using IdentityAccess.Application.Common.Interfaces;
using IdentityAccess.Application.Usecases.Auth.ForgotPassword;
using IdentityAccess.Application.Usecases.Auth.Login;
using IdentityAccess.Application.Usecases.Auth.Logout;
using IdentityAccess.Application.Usecases.Auth.RefreshToken;
using IdentityAccess.Application.Usecases.Auth.Register;
using IdentityAccess.Contracts.Auth;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityAccess.Api.Controllers.V1;

[ApiController]
[Route("api/v1/auth")]
public class AuthController(ISender sender) : BaseController(sender)
{
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(typeof(Result<KeycloakTokenResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(
        [FromBody] LoginUserRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Sender.Send(
            new LoginUserCommand(request.Username, request.Password),
            cancellationToken);

        return OkResult(result);
    }
    
    [Authorize]
    [HttpPost("logout")]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Logout(
        [FromBody] LogoutUserRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Sender.Send(
            new LogoutUserCommand(request.RefreshToken),
            cancellationToken);

        return OkResult(result);
    }
    [AllowAnonymous]
    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(Result<KeycloakTokenResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken(
        [FromBody] RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Sender.Send(
            new RefreshTokenCommand(request.RefreshToken),
            cancellationToken);

        return OkResult(result);
    }
    
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterUserRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Sender.Send(
            new RegisterUserCommand(
                request.Username,
                request.Email,
                request.FirstName,
                request.LastName,
                request.Password),
            cancellationToken);

        return CreatedResult(
            result,
            nameof(Me),
            new { });
    }
    
    
    [AllowAnonymous]
    [HttpPost("forgot-password")]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ForgotPassword(
        [FromBody] ForgotPasswordRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Sender.Send(
            new ForgotPasswordCommand(request.Email),
            cancellationToken);

        return OkResult(result);
    }
    
    
    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public IActionResult Me()
    {
        return Ok(new
        {
            UserId = User.FindFirst("sub")?.Value,
            Username = User.FindFirst("preferred_username")?.Value,
            Email = User.FindFirst("email")?.Value,
            FirstName = User.FindFirst("given_name")?.Value,
            LastName = User.FindFirst("family_name")?.Value,
            Roles = User.FindAll(ClaimTypes.Role).Select(x => x.Value).ToList()
        });
    }
    
}