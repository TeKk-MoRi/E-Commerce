using System.Security.Claims;
using ECommerce.Application.Common.Interfaces;

namespace ECommerce.Presentation.Services;


public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public Guid? UserId
    {
        get
        {
            var value = GetClaimValue(ClaimTypes.NameIdentifier) ?? GetClaimValue("sub");

            return Guid.TryParse(value, out var userId)
                ? userId
                : null;
        }
    }

    public string? Username =>
        GetClaimValue("preferred_username") ??
        GetClaimValue(ClaimTypes.Name);

    public string? Email =>
        GetClaimValue(ClaimTypes.Email) ??
        GetClaimValue("email");

    private string? GetClaimValue(string claimType)
    {
        var user = httpContextAccessor.HttpContext?.User;

        if (user?.Identity?.IsAuthenticated != true)
            return null;

        return user.FindFirstValue(claimType);
    }
}