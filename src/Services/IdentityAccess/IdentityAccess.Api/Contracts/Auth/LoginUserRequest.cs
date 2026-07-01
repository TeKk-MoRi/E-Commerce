namespace IdentityAccess.Api.Contracts.Auth;

public sealed record LoginUserRequest(
    string Username,
    string Password);