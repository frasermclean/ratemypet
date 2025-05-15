using Azure.AI.ContentSafety;
using Azure.AI.Vision.ImageAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RateMyPet.Core;
using RateMyPet.Core.Abstractions;

namespace RateMyPet.AiServices;

public static class ServiceRegistration
{
    public static IServiceCollection AddAiServices(this IServiceCollection services)
    {
        services.AddOptions<AiServicesOptions>()
            .BindConfiguration(AiServicesOptions.SectionName)
            .ValidateDataAnnotations();

        services.AddSingleton<ImageAnalysisClient>(provider =>
        {
            var options = provider.GetRequiredService<IOptions<AiServicesOptions>>();
            var endpoint = options.Value.ComputerVisionEndpoint;
            return new ImageAnalysisClient(endpoint, TokenCredentialFactory.Create());
        });

        services.AddSingleton<IImageAnalysisService, ImageAnalysisService>();

        services.AddSingleton<ContentSafetyClient>(provider =>
        {
            var options = provider.GetRequiredService<IOptions<AiServicesOptions>>();
            var endpoint = options.Value.ContentSafetyEndpoint;
            return new ContentSafetyClient(endpoint, TokenCredentialFactory.Create());
        });

        services.AddSingleton<IModerationService, ModerationService>();

        return services;
    }
}
