namespace IdentityAccess.Application.Authorization;

public static class ApplicationRoles
{
    public const string Admin = "admin";
    public const string Customer = "customer";
    
    public static readonly string[] All =
    [
        Admin,
        Customer
    ];
}