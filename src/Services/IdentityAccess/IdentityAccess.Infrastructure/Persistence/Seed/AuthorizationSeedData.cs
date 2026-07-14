using IdentityAccess.Application.Authorization;
using IdentityAccess.Domain.Authorization;
using Catalog.Contracts.Authorization;


namespace IdentityAccess.Infrastructure.Persistence.Seed;

internal static class AuthorizationSeedData
{
    internal static readonly Guid ViewProductsPermissionId =
        Guid.Parse("11111111-1111-1111-1111-111111111111");

    internal static readonly Guid ManageProductsPermissionId =
        Guid.Parse("22222222-2222-2222-2222-222222222222");

    internal static readonly Guid ViewOwnOrdersPermissionId =
        Guid.Parse("33333333-3333-3333-3333-333333333333");

    internal static readonly Guid ManageOrdersPermissionId =
        Guid.Parse("44444444-4444-4444-4444-444444444444");

    internal static readonly Guid ManageUsersPermissionId =
        Guid.Parse("55555555-5555-5555-5555-555555555555");

    internal static readonly Guid ManageRolesPermissionId =
        Guid.Parse("66666666-6666-6666-6666-666666666666");

    internal static readonly DateTime SeededAt =
        new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    internal static Permission[] Permissions =>
    [
        CreatePermission(
            ViewProductsPermissionId,
            CatalogPermissions.ProductsView,
            "Can view products."),

        CreatePermission(
            ManageProductsPermissionId,
            CatalogPermissions.ProductsManage,
            "Can create, update, and delete products."),

        CreatePermission(
            ViewOwnOrdersPermissionId,
            ApplicationPermissions.OrderingOrdersViewOwn,
            "Can view own orders."),

        CreatePermission(
            ManageOrdersPermissionId,
            ApplicationPermissions.OrderingOrdersManage,
            "Can manage all orders."),

        CreatePermission(
            ManageUsersPermissionId,
            ApplicationPermissions.IdentityUsersManage,
            "Can manage users."),

        CreatePermission(
            ManageRolesPermissionId,
            ApplicationPermissions.IdentityRolesManage,
            "Can manage roles and role permissions.")
    ];

    internal static RolePermission[] RolePermissions =>
    [
        CreateRolePermission(Guid.Parse("aaaaaaa1-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), ApplicationRoles.Admin, ViewProductsPermissionId),
        CreateRolePermission(Guid.Parse("aaaaaaa2-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), ApplicationRoles.Admin, ManageProductsPermissionId),
        CreateRolePermission(Guid.Parse("aaaaaaa3-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), ApplicationRoles.Admin, ViewOwnOrdersPermissionId),
        CreateRolePermission(Guid.Parse("aaaaaaa4-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), ApplicationRoles.Admin, ManageOrdersPermissionId),
        CreateRolePermission(Guid.Parse("aaaaaaa5-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), ApplicationRoles.Admin, ManageUsersPermissionId),
        CreateRolePermission(Guid.Parse("aaaaaaa6-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), ApplicationRoles.Admin, ManageRolesPermissionId),

        CreateRolePermission(Guid.Parse("bbbbbbb1-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), ApplicationRoles.Customer, ViewProductsPermissionId),
        CreateRolePermission(Guid.Parse("bbbbbbb2-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), ApplicationRoles.Customer, ViewOwnOrdersPermissionId)
    ];

    private static Permission CreatePermission(Guid id, string code, string description)
    {
        var permission = Permission.Create(code, description);

        typeof(Permission)
            .GetProperty(nameof(Permission.Id))!
            .SetValue(permission, id);

        typeof(Permission)
            .GetProperty(nameof(Permission.CreatedAt))!
            .SetValue(permission, SeededAt);

        return permission;
    }

    private static RolePermission CreateRolePermission(Guid id, string roleName, Guid permissionId)
    {
        var rolePermission = RolePermission.Create(roleName, permissionId);

        typeof(RolePermission)
            .GetProperty(nameof(RolePermission.Id))!
            .SetValue(rolePermission, id);

        typeof(RolePermission)
            .GetProperty(nameof(RolePermission.CreatedAt))!
            .SetValue(rolePermission, SeededAt);

        return rolePermission;
    }
}