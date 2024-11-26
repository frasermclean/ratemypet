using Azure.Storage.Blobs;
using RateMyPet.Persistence;

namespace RateMyPet.Api.Extensions;

public static class BlobServiceClientExtensions
{
    public static Uri GetBlobUri(this BlobServiceClient blobServiceClient, string blobName,
        string containerName = BlobContainerNames.OriginalImages) => blobServiceClient
        .GetBlobContainerClient(containerName)
        .GetBlobClient(blobName).Uri;
}
