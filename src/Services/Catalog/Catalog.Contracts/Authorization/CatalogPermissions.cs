namespace Catalog.Contracts.Authorization;

public static class CatalogPermissions
{
    public const string ProductsView = "catalog.products.view";

    public const string ProductsManage = "catalog.products.manage";

    public static readonly string[] All =
    [
        ProductsView,
        ProductsManage
    ];
}