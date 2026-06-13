using ECommerce.Application.Common;
using ECommerce.Application.Common.Interfaces;
using MediatR;

namespace ECommerce.Application.Usecases.Auth.RefreshToken;

public sealed record RefreshTokenCommand(string RefreshToken)
    : IRequest<Result<KeycloakTokenResponse>>;

public sealed class RefreshTokenCommandHandler(IKeycloakService keycloakService)
    : IRequestHandler<RefreshTokenCommand, Result<KeycloakTokenResponse>>
{
    private readonly IKeycloakService _keycloakService = keycloakService;

    public async Task<Result<KeycloakTokenResponse>> Handle(
        RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        return await _keycloakService.RefreshTokenAsync(request.RefreshToken);
    }
}