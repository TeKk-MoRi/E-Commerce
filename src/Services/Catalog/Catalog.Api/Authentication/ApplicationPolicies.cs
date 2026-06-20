namespace Catalog.Api.Authentication;

public static class ApplicationPolicies
{
    public const string CanViewProducts = "CanViewProducts";
    public const string CanManageProducts = "CanManageProducts";

    public const string CanViewOwnOrders = "CanViewOwnOrders";
    public const string CanManageOrders = "CanManageOrders";

    public const string CanManageUsers = "CanManageUsers";
    public const string CanManageRoles = "CanManageRoles";
}