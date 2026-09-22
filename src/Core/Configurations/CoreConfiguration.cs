using Core.Interfaces;
using Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Configurations;

public static class CoreConfiguration
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICategoryService, CategoryService>();

        return services;
    }
}
