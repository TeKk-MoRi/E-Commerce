namespace ECommerce.Presentation.Authentication;

public static class ApplicationPermissions
{
    public const string ViewProducts = "products:view";
    public const string ManageProducts = "products:manage";

    public const string ViewOwnOrders = "orders:view-own";
    public const string ManageOrders = "orders:manage";

    public const string ManageUsers = "users:manage";
    public const string ManageRoles = "roles:manage";

    public static readonly string[] All =
    [
        ViewProducts,
        ManageProducts,
        ViewOwnOrders,
        ManageOrders,
        ManageUsers,
        ManageRoles
    ];
}