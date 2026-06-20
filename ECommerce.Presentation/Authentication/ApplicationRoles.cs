namespace ECommerce.Presentation.Authentication;

public static class ApplicationRoles
{
    public const string Admin = "admin";
    public const string Customer = "customer";
    
    public static readonly IReadOnlyCollection<string> All =
    [
        Admin,
        Customer
    ];
}