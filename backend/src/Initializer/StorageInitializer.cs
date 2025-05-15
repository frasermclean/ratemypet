using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using RateMyPet.Storage;

namespace RateMyPet.Initializer;

public class StorageInitializer(
    ILogger<StorageInitializer> logger,
    BlobServiceClient blobServiceClient,
    QueueServiceClient queueServiceClient)
{
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await InitializeBlobContainersAsync(cancellationToken);
        await InitializeQueuesAsync(cancellationToken);
    }

    private async Task InitializeBlobContainersAsync(CancellationToken cancellationToken)
    {
        var postImagesContainerClient = blobServiceClient.GetBlobContainerClient(BlobContainerNames.PostImages);
        await postImagesContainerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

        logger.LogInformation("Blob containers initialized");
    }

    private async Task InitializeQueuesAsync(CancellationToken cancellationToken)
    {
        var queuesToCreates = new[]
        {
            QueueNames.ForgotPassword, QueueNames.PostAdded, QueueNames.RegisterConfirmation
        };

        foreach (var queueName in queuesToCreates)
        {
            var queueClient = queueServiceClient.GetQueueClient(queueName);
            await queueClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

            logger.LogInformation("Queue {QueueName} initialized", queueName);
        }
    }
}
