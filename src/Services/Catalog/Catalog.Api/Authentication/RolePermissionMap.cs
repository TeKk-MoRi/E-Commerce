namespace Catalog.Api.Authentication;

public static class RolePermissionMap
{
    public static readonly Dictionary<string, string[]> PermissionsByRole = new()
    {
        [ApplicationRoles.Admin] =
        [
            ApplicationPermissions.ViewProducts,
            ApplicationPermissions.ManageProducts,
            ApplicationPermissions.ViewOwnOrders,
            ApplicationPermissions.ManageOrders,
            ApplicationPermissions.ManageUsers,
            ApplicationPermissions.ManageRoles
        ],

        [ApplicationRoles.Customer] =
        [
            ApplicationPermissions.ViewProducts,
            ApplicationPermissions.ViewOwnOrders
        ]
    };
}