namespace IdentityAccess.Application.Authorization.DTOs;

public sealed record PermissionDto(
    Guid Id,
    string Code,
    string Description);