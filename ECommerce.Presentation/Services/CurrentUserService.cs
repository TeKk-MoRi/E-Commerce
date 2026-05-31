using System.Security.Claims;
using ECommerce.Application.Common.Interfaces;

namespace ECommerce.Presentation.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public Guid? UserId
    {
        get
        {
            var user = httpContextAccessor.HttpContext?.User;

            if (user?.Identity?.IsAuthenticated != true)

                return null;

            var userIdValue =
                user.FindFirstValue(ClaimTypes.NameIdentifier) ??
                user.FindFirstValue("sub");

            return Guid.TryParse(userIdValue, out var userId)
                ? userId
                : null;
        }
    }
}