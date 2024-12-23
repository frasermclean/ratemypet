using Microsoft.Extensions.DependencyInjection;

namespace RateMyPet.Logic.Services;

public static class ServiceRegistration
{
    public static IServiceCollection AddLogicServices(this IServiceCollection services)
    {
        services.AddTransient<ImageProcessor>();

        return services;
    }
}
