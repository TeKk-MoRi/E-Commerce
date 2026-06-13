using ECommerce.Application.Common;
using ECommerce.Application.Common.Interfaces;
using MediatR;

namespace ECommerce.Application.Usecases.Auth.Logout;

public sealed record LogoutUserCommand(string RefreshToken)
    : IRequest<Result<bool>>;

public sealed class LogoutUserCommandHandler(IKeycloakService keycloakService)
    : IRequestHandler<LogoutUserCommand, Result<bool>>
{
    private readonly IKeycloakService _keycloakService = keycloakService;

    public async Task<Result<bool>> Handle(
        LogoutUserCommand request,
        CancellationToken cancellationToken)
    {
        return await _keycloakService.LogoutAsync(request.RefreshToken);
    }
}