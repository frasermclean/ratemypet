using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RateMyPet.Core.Abstractions;
using RateMyPet.Infrastructure.Options;
using RateMyPet.Infrastructure.Services.ImageProcessing;

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
            .AddTransient<IPostImageProcessor, PostImageProcessor>()
            .AddTransient<IEmailSender, EmailSender>()
            .AddSingleton<IMessagePublisher, MessagePublisher>();

        services.AddOptions<EmailOptions>()
            .BindConfiguration(EmailOptions.SectionName);

        services.AddOptions<ImageProcessingOptions>()
            .BindConfiguration(ImageProcessingOptions.SectionName)
            .ValidateDataAnnotations();

        return services;
    }

    private static IServiceCollection AddAzureClients(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAzureClients(factoryBuilder =>
        {
            // use connection string if endpoints are not provided
            var blobEndpoint = configuration["Storage:BlobEndpoint"];
            var queueEndpoint = configuration["Storage:QueueEndpoint"];
            if (string.IsNullOrEmpty(blobEndpoint) || string.IsNullOrEmpty(queueEndpoint))
            {
                var connectionString = configuration.GetConnectionString("Storage");
                factoryBuilder.AddBlobServiceClient(connectionString);
                factoryBuilder.AddQueueServiceClient(connectionString);
            }
            else
            {
                factoryBuilder.AddBlobServiceClient(new Uri(blobEndpoint));
                factoryBuilder.AddQueueServiceClient(new Uri(queueEndpoint));
            }

            factoryBuilder.AddEmailClient(new Uri(configuration["Email:AcsEndpoint"]!));
            factoryBuilder.UseCredential(TokenCredentialFactory.Create());
            factoryBuilder.ConfigureDefaults(options => options.Diagnostics.IsLoggingEnabled = false);
        });

        return services;
    }

    private static IServiceCollection AddBlobContainerManagers(this IServiceCollection services)
    {
        services.AddKeyedScoped<IBlobContainerManager>(BlobContainerNames.Images, (provider, _) =>
            new BlobContainerManager(provider.GetRequiredService<BlobServiceClient>()
                .GetBlobContainerClient(BlobContainerNames.Images)));

        services.AddKeyedScoped<IBlobContainerManager>(BlobContainerNames.OriginalImages, (provider, _) =>
            new BlobContainerManager(provider.GetRequiredService<BlobServiceClient>()
                .GetBlobContainerClient(BlobContainerNames.OriginalImages)));

        services.AddKeyedScoped<IBlobContainerManager>(BlobContainerNames.PostImages, (provider, _) =>
            new BlobContainerManager(provider.GetRequiredService<BlobServiceClient>()
                .GetBlobContainerClient(BlobContainerNames.PostImages)));

        return services;
    }
}
