using IdentityAccess.Application.Common;
using IdentityAccess.Application.Common.Enums;
using IdentityAccess.Application.Common.Interfaces;
using MediatR;

namespace IdentityAccess.Application.Usecases.Auth.Register;

public sealed record RegisterUserCommand(
    string Username,
    string Email,
    string FirstName,
    string LastName,
    string Password) : IRequest<Result<RegisterUserResponse>>;

public sealed record RegisterUserResponse(
    string UserId,
    string Username,
    string Email,
    string FirstName,
    string LastName);

public sealed class RegisterUserCommandHandler(IKeycloakService keycloakService)
    : IRequestHandler<RegisterUserCommand, Result<RegisterUserResponse>>
{
    private readonly IKeycloakService _keycloakService = keycloakService;

    public async Task<Result<RegisterUserResponse>> Handle(
        RegisterUserCommand request,
        CancellationToken cancellationToken)
    {
        var existingUserResult = await _keycloakService.GetUserByEmailAsync(request.Email);

        if (existingUserResult.IsSuccess)
        {
            return Result<RegisterUserResponse>.Conflict(
                "User.EmailAlreadyExists",
                "A user with this email already exists.");
        }

        if (existingUserResult.Error.Type != ErrorType.NotFound)
        {
            return Result<RegisterUserResponse>.Failure(existingUserResult.Error);
        }

        var createdUserIdResult = await _keycloakService.CreateUserAsync(
            request.Username,
            request.Email,
            request.FirstName,
            request.LastName,
            request.Password);

        if (createdUserIdResult.IsFailure)
            return Result<RegisterUserResponse>.Failure(createdUserIdResult.Error);

        var response = new RegisterUserResponse(
            createdUserIdResult.Data!,
            request.Username,
            request.Email,
            request.FirstName,
            request.LastName);

        return Result<RegisterUserResponse>.Success(
            response,
            "User registered successfully.");
    }
}