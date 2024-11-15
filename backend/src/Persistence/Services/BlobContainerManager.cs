using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace RateMyPet.Persistence.Services;

public interface IBlobContainerManager
{
    Task UploadBlobAsync(string blobName, Stream stream, string contentType,
        CancellationToken cancellationToken = default);
}

public class BlobContainerManager(BlobContainerClient containerClient) : IBlobContainerManager
{
    public async Task UploadBlobAsync(string blobName, Stream stream, string contentType,
        CancellationToken cancellationToken)
    {
        var blobClient = containerClient.GetBlobClient(blobName);
        var options = new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders { ContentType = contentType }
        };

        await blobClient.UploadAsync(stream, options, cancellationToken);
    }
}
