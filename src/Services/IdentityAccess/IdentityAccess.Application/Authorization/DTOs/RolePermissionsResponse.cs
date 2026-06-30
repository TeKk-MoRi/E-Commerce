namespace IdentityAccess.Application.Authorization.DTOs;

public sealed record RolePermissionsResponse(
    string RoleName,
    IReadOnlyCollection<PermissionResponse> Permissions);