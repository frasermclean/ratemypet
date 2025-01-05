using Azure.Storage.Blobs;
using SixLabors.ImageSharp.Web.Resolvers;

namespace RateMyPet.Infrastructure.Services.ImageProcessing;

public class BlobStorageImageCacheResolver(BlobClient blobClient) : IImageCacheResolver
{
    public async Task<ImageCacheMetadata> GetMetaDataAsync()
    {
        var propertiesResponse = await blobClient.GetPropertiesAsync();
        return ImageCacheMetadata.FromDictionary(propertiesResponse.Value.Metadata);

    }

    public Task<Stream> OpenReadAsync() => blobClient.OpenReadAsync();
}
