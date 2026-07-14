using Catalog.Contracts.Authorization;

namespace IdentityAccess.Application.Authorization;

public static class ApplicationPermissions
{
    public const string OrderingOrdersViewOwn =
        "ordering.orders.view-own";

    public const string OrderingOrdersManage =
        "ordering.orders.manage";

    public const string IdentityUsersManage =
        "identity.users.manage";

    public const string IdentityRolesManage =
        "identity.roles.manage";

    public static readonly string[] All =
    [
        ..CatalogPermissions.All,
        OrderingOrdersViewOwn,
        OrderingOrdersManage,
        IdentityUsersManage,
        IdentityRolesManage
    ];
}