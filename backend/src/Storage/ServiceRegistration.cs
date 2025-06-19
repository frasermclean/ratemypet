using Azure.Storage.Blobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RateMyPet.Core.Abstractions;

namespace RateMyPet.Storage;

public static class ServiceRegistration
{
    public static TBuilder AddStorageServices<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        builder.AddAzureBlobClient("blobs");
        builder.AddAzureQueueClient("queues");

        builder.Services
            .AddSingleton<IMessagePublisher, MessagePublisher>()
            .AddBlobContainerManagers();

        return builder;
    }

    private static void AddBlobContainerManagers(this IServiceCollection services)
    {
        services.AddKeyedScoped<IBlobContainerManager>(BlobContainerNames.PostImages, (provider, _) =>
            new BlobContainerManager(provider.GetRequiredService<BlobServiceClient>()
                .GetBlobContainerClient(BlobContainerNames.PostImages)));
    }
}
