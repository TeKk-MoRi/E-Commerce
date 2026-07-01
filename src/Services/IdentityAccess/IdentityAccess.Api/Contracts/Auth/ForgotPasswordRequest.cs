namespace IdentityAccess.Api.Contracts.Auth;

public sealed record ForgotPasswordRequest(
    string Email);