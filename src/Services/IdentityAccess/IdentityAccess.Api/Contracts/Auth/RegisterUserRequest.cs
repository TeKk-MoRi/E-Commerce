namespace IdentityAccess.Api.Contracts.Auth;

public sealed record RegisterUserRequest(
    string Username,
    string Email,
    string FirstName,
    string LastName,
    string Password);