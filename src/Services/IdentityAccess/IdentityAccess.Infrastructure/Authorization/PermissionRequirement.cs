using Microsoft.AspNetCore.Authorization;

namespace IdentityAccess.Infrastructure.Authorization;

public sealed class PermissionRequirement(string permission) : IAuthorizationRequirement
{
    public string Permission { get; } = permission;
}