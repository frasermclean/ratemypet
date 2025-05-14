using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RateMyPet.Core.Abstractions;
using RateMyPet.Infrastructure.Services.Email;
using RateMyPet.Infrastructure.Services.ImageAnalysis;
using RateMyPet.Infrastructure.Services.Moderation;

namespace RateMyPet.Infrastructure.Services;

public static class ServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAzureClients(configuration)
            .AddImageAnalysis()
            .AddModeration()
            .AddEmailSending();

        return services;
    }

    private static IServiceCollection AddAzureClients(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAzureClients(builder =>
        {
            // ai services
            builder.AddImageAnalysisClient(configuration.GetValue<Uri>("AiServices:ComputerVisionEndpoint"));
            builder.AddContentSafetyClient(configuration.GetValue<Uri>("AiServices:ContentSafetyEndpoint"));

            builder.AddEmailClient(configuration.GetValue<Uri>("Email:AcsEndpoint"));

            builder.UseCredential(TokenCredentialFactory.Create());
            builder.ConfigureDefaults(options => options.Diagnostics.IsLoggingEnabled = false);
        });

        return services;
    }

    private static IServiceCollection AddImageAnalysis(this IServiceCollection services)
    {
        services.AddOptions<ImageAnalysisOptions>()
            .BindConfiguration(ImageAnalysisOptions.SectionName)
            .ValidateDataAnnotations();

        services.AddSingleton<IImageAnalysisService, ImageAnalysisService>();

        return services;
    }

    private static IServiceCollection AddModeration(this IServiceCollection services)
    {
        services.AddOptions<ModerationOptions>()
            .BindConfiguration(ModerationOptions.SectionName)
            .ValidateDataAnnotations();

        services.AddSingleton<IModerationService, ModerationService>();

        return services;
    }

    private static IServiceCollection AddEmailSending(this IServiceCollection services)
    {
        services.AddOptions<EmailOptions>()
            .BindConfiguration(EmailOptions.SectionName)
            .ValidateDataAnnotations();

        services.AddTransient<IEmailSender, EmailSender>();

        return services;
    }
}
