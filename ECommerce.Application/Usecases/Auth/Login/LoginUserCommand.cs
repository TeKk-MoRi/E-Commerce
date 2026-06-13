using ECommerce.Application.Common;
using ECommerce.Application.Common.Interfaces;
using MediatR;

namespace ECommerce.Application.Usecases.Auth.login;

public sealed record LoginUserCommand(
    string Username,
    string Password) : IRequest<Result<KeycloakTokenResponse>>;

public sealed class LoginUserCommandHandler(IKeycloakService keycloakService)
    : IRequestHandler<LoginUserCommand, Result<KeycloakTokenResponse>>
{
    private readonly IKeycloakService _keycloakService = keycloakService;

    public async Task<Result<KeycloakTokenResponse>> Handle(
        LoginUserCommand request,
        CancellationToken cancellationToken)
    {
        return await _keycloakService.LoginAsync(request.Username, request.Password);
    }
}