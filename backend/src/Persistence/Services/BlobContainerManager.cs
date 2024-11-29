using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace RateMyPet.Persistence.Services;

public interface IBlobContainerManager
{
    Task<Stream> OpenWriteStreamAsync(string blobName, string contentType,
        CancellationToken cancellationToken = default);

    Task DeleteBlobAsync(string blobName, CancellationToken cancellationToken = default);
}

public class BlobContainerManager(BlobContainerClient containerClient) : IBlobContainerManager
{
    public Task<Stream> OpenWriteStreamAsync(string blobName, string contentType,
        CancellationToken cancellationToken = default)
    {
        var blobClient = containerClient.GetBlobClient(blobName);
        var options = new BlobOpenWriteOptions
        {
            HttpHeaders = new BlobHttpHeaders { ContentType = contentType }
        };

        return blobClient.OpenWriteAsync(true, options, cancellationToken);
    }

    public async Task DeleteBlobAsync(string blobName, CancellationToken cancellationToken = default)
    {
        var blobClient = containerClient.GetBlobClient(blobName);
        await blobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots,
            cancellationToken: cancellationToken);
    }
}
