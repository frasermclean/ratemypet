namespace RateMyPet.Core.Abstractions;

public interface IBlobContainerManager
{
    Task<Stream> OpenReadStreamAsync(string blobName, CancellationToken cancellationToken = default);

    Task<Stream> OpenWriteStreamAsync(string blobName, string contentType,
        CancellationToken cancellationToken = default);

    Task CreateBlobAsync(string blobName, Stream content, string contentType,
        CancellationToken cancellationToken = default);

    Task DeleteBlobAsync(string blobName, CancellationToken cancellationToken = default);
}
