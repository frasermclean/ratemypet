using System.Net.Http.Headers;
using Azure.Storage.Blobs;
using SixLabors.ImageSharp.Web.Resolvers;

namespace RateMyPet.Infrastructure.Services.ImageProcessing;

public class BlobStorageImageResolver(BlobClient blobClient) : IImageResolver
{
    public async Task<ImageMetadata> GetMetaDataAsync()
    {
        var propertiesResponse = await blobClient.GetPropertiesAsync();
        var properties = propertiesResponse.Value;

        var maxAge = CacheControlHeaderValue.TryParse(properties.CacheControl, out var cacheControl)
            ? cacheControl.MaxAge ?? TimeSpan.MinValue
            : TimeSpan.MinValue;

        return new ImageMetadata(properties.LastModified.UtcDateTime, maxAge, properties.ContentLength);
    }

    public Task<Stream> OpenReadAsync() => blobClient.OpenReadAsync();
}
