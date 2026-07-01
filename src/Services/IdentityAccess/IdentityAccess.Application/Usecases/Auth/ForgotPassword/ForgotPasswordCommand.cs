using ECommerce.BuildingBlocks.Application;
using IdentityAccess.Application.Common.Interfaces;
using MediatR;

namespace IdentityAccess.Application.Usecases.Auth.ForgotPassword;

public sealed record ForgotPasswordCommand(string Email)
    : IRequest<Result<bool>>;

public sealed class ForgotPasswordCommandHandler(
    IKeycloakService keycloakService)
    : IRequestHandler<ForgotPasswordCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        ForgotPasswordCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return Result<bool>.ValidationFailure(
                "Auth.EmailRequired",
                "Email is required.");
        }

        return await keycloakService.SendPasswordResetEmailAsync(
            request.Email.Trim(),
            cancellationToken);
    }
}