using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RateMyPet.Jobs.Options;
using RateMyPet.Jobs.Services;
using RateMyPet.Persistence;
using RateMyPet.Persistence.Services;

namespace RateMyPet.Jobs.Startup;

public static class ServiceRegistration
{
    public static FunctionsApplicationBuilder RegisterServices(this FunctionsApplicationBuilder builder)
    {
        builder.Services
            .AddApplicationInsightsTelemetryWorkerService()
            .ConfigureFunctionsApplicationInsights()
            .AddPersistence(builder.Configuration)
            .AddJobsServices();

        return builder;
    }

    private static IServiceCollection AddPersistence(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddDbContextFactory<ApplicationDbContext>(builder =>
        {
            var connectionString = configuration.GetConnectionString("Database");
            builder.UseSqlServer(connectionString);
        });

        services.AddAzureClients(factoryBuilder =>
        {
            factoryBuilder.AddEmailClient(new Uri(configuration["Email:AcsEndpoint"]!));
            factoryBuilder.AddBlobServiceClient(new Uri(configuration["Storage:BlobEndpoint"]!));
            factoryBuilder.AddQueueServiceClient(new Uri(configuration["Storage:QueueEndpoint"]!));
            factoryBuilder.UseCredential(TokenCredentialFactory.Create());
            factoryBuilder.ConfigureDefaults(options => options.Diagnostics.IsLoggingEnabled = false);
        });

        services.AddKeyedScoped<IBlobContainerManager>(BlobContainerNames.OriginalImages, (provider, _) =>
            new BlobContainerManager(provider.GetRequiredService<BlobServiceClient>()
                .GetBlobContainerClient(BlobContainerNames.OriginalImages)));

        services.AddKeyedScoped<IBlobContainerManager>(BlobContainerNames.PostImages, (provider, _) =>
            new BlobContainerManager(provider.GetRequiredService<BlobServiceClient>()
                .GetBlobContainerClient(BlobContainerNames.PostImages)));

        return services;
    }

    private static void AddJobsServices(this IServiceCollection services)
    {
        services.AddOptions<EmailOptions>().BindConfiguration(EmailOptions.SectionName);
        services.AddTransient<IEmailSender, EmailSender>();
    }
}
