namespace IdentityAccess.Application.Authorization.DTOs;

public sealed record PermissionResponse(
    string Code,
    string Description);