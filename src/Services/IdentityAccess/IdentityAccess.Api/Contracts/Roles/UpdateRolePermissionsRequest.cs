namespace IdentityAccess.Api.Contracts.Roles;

public sealed record UpdateRolePermissionsRequest(
    IReadOnlyCollection<string> PermissionCodes);