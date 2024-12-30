using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RateMyPet.Logic.Options;
using RateMyPet.Persistence;

namespace RateMyPet.Logic.Services;

public static class ServiceRegistration
{
    public static IServiceCollection AddLogicServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAzureClients(configuration)
            .AddTransient<IPostImageProcessor, PostImageProcessor>()
            .AddTransient<IEmailSender, EmailSender>();

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
}
