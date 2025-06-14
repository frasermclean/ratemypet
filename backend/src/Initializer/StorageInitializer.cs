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

        var postImagesContainerClient = blobServiceClient.GetBlobContainerClient(BlobContainerNames.PostImages);
        await postImagesContainerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

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
