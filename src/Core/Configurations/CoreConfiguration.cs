using Microsoft.Extensions.DependencyInjection;

namespace Core.Configurations;

public static class CoreConfiguration
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        //services.AddScoped<ITaskService, TaskService>();
        //services.AddScoped<ICategoryService, CategoryService>();

        return services;
    }
}