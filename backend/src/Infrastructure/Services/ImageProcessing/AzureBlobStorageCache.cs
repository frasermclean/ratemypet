using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp.Web.Caching;
using SixLabors.ImageSharp.Web.Resolvers;

namespace RateMyPet.Infrastructure.Services.ImageProcessing;

public class AzureBlobStorageCache(BlobServiceClient blobServiceClient, IOptions<ImageProcessingOptions> options)
    : IImageCache
{
    private readonly BlobContainerClient containerClient =
        blobServiceClient.GetBlobContainerClient(options.Value.CacheContainerName);

    public async Task<IImageCacheResolver?> GetAsync(string key)
    {
        var blobClient = containerClient.GetBlobClient(key);

        return await blobClient.ExistsAsync()
            ? new AzureBlobStorageCacheResolver(blobClient)
            : null;
    }

    public async Task SetAsync(string key, Stream stream, ImageCacheMetadata metadata)
    {
        var blobClient = containerClient.GetBlobClient(key);
        var headers = new BlobHttpHeaders
        {
            ContentType = metadata.ContentType
        };

        await blobClient.UploadAsync(stream, headers, metadata.ToDictionary());
    }
}
