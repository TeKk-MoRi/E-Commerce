using Catalog.Application.Common.Interfaces;
using Catalog.Api.Services;

namespace Catalog.Api
{
    public static class ConfigureServices
    {
        public static IServiceCollection RegisterPresentationServices(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            return services;
        }
    }
}