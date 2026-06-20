using Catalog.Application.Common.Interfaces;
using Catalog.Api.Authentication;
using Catalog.Api.Services;
using Microsoft.AspNetCore.Authentication;

namespace Catalog.Api
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
