namespace IdentityAccess.Domain.Authorization;

public sealed class RolePermission
{
    private RolePermission()
    {
    }

    private RolePermission(string roleName, Guid permissionId)
    {
        Id = Guid.NewGuid();
        RoleName = roleName;
        PermissionId = permissionId;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }

    public string RoleName { get; private set; } = null!;

    public Guid PermissionId { get; private set; }

    public Permission Permission { get; private set; } = null!;

    public DateTime CreatedAt { get; private set; }

    public static RolePermission Create(string roleName, Guid permissionId)
    {
        if (string.IsNullOrWhiteSpace(roleName))
            throw new ArgumentException("Role name cannot be empty.", nameof(roleName));

        if (permissionId == Guid.Empty)
            throw new ArgumentException("Permission id cannot be empty.", nameof(permissionId));

        return new RolePermission(roleName.Trim(), permissionId);
    }
}