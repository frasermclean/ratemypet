using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RateMyPet.Persistence;
using RateMyPet.Persistence.Services;

namespace RateMyPet.Jobs.Startup;

public static class ServiceRegistration
{
    public static FunctionsApplicationBuilder RegisterServices(this FunctionsApplicationBuilder builder)
    {
        builder.Services
            .AddApplicationInsightsTelemetryWorkerService()
            .ConfigureFunctionsApplicationInsights();

        builder.Services.AddPersistence(builder.Configuration);

        return builder;
    }

    private static IServiceCollection AddPersistence(this IServiceCollection services,
        ConfigurationManager configuration)
    {
        services.AddDbContextFactory<ApplicationDbContext>(builder =>
        {
            var connectionString = configuration.GetConnectionString("Database");
            builder.UseSqlServer(connectionString);
        });

        services.AddKeyedScoped<IBlobContainerManager>(BlobContainerNames.PostImages, (provider, _) =>
        {
            var containerClient = provider.GetRequiredService<BlobServiceClient>()
                .GetBlobContainerClient(BlobContainerNames.PostImages);
            return new BlobContainerManager(containerClient);
        });

        services.AddAzureClients(factoryBuilder =>
        {
            factoryBuilder.AddBlobServiceClient(new Uri(configuration["Storage:BlobEndpoint"]!));
            factoryBuilder.UseCredential(TokenCredentialFactory.Create());
            factoryBuilder.ConfigureDefaults(options => options.Diagnostics.IsLoggingEnabled = false);
        });

        return services;
    }
}
