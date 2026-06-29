namespace IdentityAccess.Application.Authorization;

public static class ApplicationPermissions
{
    public const string CatalogProductsView = "catalog.products.view";
    public const string CatalogProductsManage = "catalog.products.manage";

    public const string OrderingOrdersViewOwn = "ordering.orders.view-own";
    public const string OrderingOrdersManage = "ordering.orders.manage";

    public const string IdentityUsersManage = "identity.users.manage";
    public const string IdentityRolesManage = "identity.roles.manage";
}