using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using OpenTelemetry.Trace;
using RateMyPet.Storage;

namespace RateMyPet.Initializer;

public class StorageInitializer(
    ILogger<StorageInitializer> logger,
    Tracer tracer,
    BlobServiceClient blobServiceClient,
    QueueServiceClient queueServiceClient)
{
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await Task.WhenAll(
            InitializeBlobContainersAsync(cancellationToken),
            InitializeQueuesAsync(cancellationToken));
    }

    private async Task InitializeBlobContainersAsync(CancellationToken cancellationToken)
    {
        using var span = tracer.StartActiveSpan("Initialize blob containers");

        var blobContainersToCreate = BlobContainerNames.All;
        span.SetAttribute("BlobContainersToCreate", string.Join(",", blobContainersToCreate));

        foreach (var containerName in blobContainersToCreate)
        {
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

            logger.LogInformation("Blob container {ContainerName} initialized", containerName);
        }

        logger.LogInformation("Blob containers initialized");
    }

    private async Task InitializeQueuesAsync(CancellationToken cancellationToken)
    {
        using var span = tracer.StartActiveSpan("Initialize queues");

        var queuesToCreate = new[]
        {
            QueueNames.ForgotPassword, QueueNames.PostAdded, QueueNames.PostDeleted, QueueNames.RegisterConfirmation
        };

        span.SetAttribute("QueuesToCreate", string.Join(",", queuesToCreate));

        foreach (var queueName in queuesToCreate)
        {
            var queueClient = queueServiceClient.GetQueueClient(queueName);
            await queueClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

            logger.LogInformation("Queue {QueueName} initialized", queueName);
        }
    }
}
