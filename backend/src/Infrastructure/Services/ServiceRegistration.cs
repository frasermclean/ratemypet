using Azure.Communication.Email;
using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RateMyPet.Core.Abstractions;
using RateMyPet.Infrastructure.Services.Email;
using RateMyPet.Infrastructure.Services.ImageAnalysis;
using RateMyPet.Infrastructure.Services.ImageHosting;

namespace RateMyPet.Infrastructure.Services;

public static class ServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        // db context factory
        services.AddDbContextFactory<ApplicationDbContext>(builder =>
        {
            var connectionString = configuration.GetConnectionString("Database");
            builder.UseSqlServer(connectionString);
        });

        services.AddAzureClients(configuration)
            .AddBlobContainerManagers()
            .AddImageHosting()
            .AddImageAnalysis()
            .AddEmailSending()
            .AddSingleton<IMessagePublisher, MessagePublisher>();

        return services;
    }

    private static IServiceCollection AddAzureClients(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAzureClients(builder =>
        {
            // use connection string if endpoints are not provided
            var blobEndpoint = configuration["Storage:BlobEndpoint"];
            var queueEndpoint = configuration["Storage:QueueEndpoint"];
            if (string.IsNullOrEmpty(blobEndpoint) || string.IsNullOrEmpty(queueEndpoint))
            {
                var connectionString = configuration.GetConnectionString("Storage");
                builder.AddBlobServiceClient(connectionString);
                builder.AddQueueServiceClient(connectionString);
            }
            else
            {
                builder.AddBlobServiceClient(new Uri(blobEndpoint));
                builder.AddQueueServiceClient(new Uri(queueEndpoint));
            }

            builder.AddImageAnalysisClient(configuration.GetValue<Uri>("CognitiveServices:Endpoint"));
            builder.AddEmailClient(configuration.GetValue<Uri>("Email:AcsEndpoint"));
            builder.UseCredential(TokenCredentialFactory.Create());
            builder.ConfigureDefaults(options => options.Diagnostics.IsLoggingEnabled = false);
        });

        return services;
    }

    private static IServiceCollection AddBlobContainerManagers(this IServiceCollection services)
    {
        services.AddKeyedScoped<IBlobContainerManager>(BlobContainerNames.Images, (provider, _) =>
            new BlobContainerManager(provider.GetRequiredService<BlobServiceClient>()
                .GetBlobContainerClient(BlobContainerNames.Images)));

        services.AddKeyedScoped<IBlobContainerManager>(BlobContainerNames.ImagesCache, (provider, _) =>
            new BlobContainerManager(provider.GetRequiredService<BlobServiceClient>()
                .GetBlobContainerClient(BlobContainerNames.ImagesCache)));

        return services;
    }

    private static IServiceCollection AddImageHosting(this IServiceCollection services)
    {
        services.AddOptions<CloudinaryOptions>()
            .BindConfiguration(CloudinaryOptions.SectionName)
            .ValidateDataAnnotations();

        services.AddHttpClient<IImageHostingService, ImageHostingService>();

        return services;
    }

    private static IServiceCollection AddImageAnalysis(this IServiceCollection services)
    {
        services.AddOptions<CognitiveServicesOptions>()
            .BindConfiguration(CognitiveServicesOptions.SectionName);

        services.AddSingleton<IImageAnalysisService, ImageAnalysisService>();

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
