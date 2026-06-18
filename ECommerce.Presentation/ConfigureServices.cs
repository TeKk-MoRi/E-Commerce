using ECommerce.Application.Common.Interfaces;
using ECommerce.Presentation.Authentication;
using ECommerce.Presentation.Services;
using Microsoft.AspNetCore.Authentication;

namespace ECommerce.Presentation
{
    public static class ConfigureServices
    {
        public static IServiceCollection RegisterPresentationServices(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            
            services.AddScoped<IClaimsTransformation, KeycloakClaimsTransformation>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            
            return services;
        }
    }
}
