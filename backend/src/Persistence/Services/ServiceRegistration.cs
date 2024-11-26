using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RateMyPet.Persistence.Services;

public static class ServiceRegistration
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContextFactory<ApplicationDbContext>(builder =>
        {
            var connectionString = configuration.GetConnectionString("Database");
            builder.UseSqlServer(connectionString);
        });

        services.AddKeyedScoped<IBlobContainerManager>(BlobContainerNames.OriginalImages, (provider, _) =>
        {
            var containerClient = provider.GetRequiredService<BlobServiceClient>()
                .GetBlobContainerClient(BlobContainerNames.OriginalImages);
            return new BlobContainerManager(containerClient);
        });

        return services;
    }
}
