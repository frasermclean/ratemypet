using Microsoft.Extensions.DependencyInjection;

namespace RateMyPet.ImageHosting;

public static class ServiceRegistration
{
    public static IServiceCollection AddImageHosting(this IServiceCollection services)
    {
        services.AddOptions<CloudinaryOptions>()
            .BindConfiguration(CloudinaryOptions.SectionName)
            .ValidateDataAnnotations();

        services.AddHttpClient<IImageHostingService, CloudinaryService>();

        return services;
    }
}
