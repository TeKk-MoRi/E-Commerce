namespace IdentityAccess.Application.Authorization;

public sealed record PermissionDto(
    Guid Id,
    string Code,
    string Description);