using RateMyPet.Core;

namespace RateMyPet.ImageHosting;

public interface IImageHostingService
{
    Task<PostImage> GetAsync(string publicId, CancellationToken cancellationToken = default);

    Uri GetPublicUri(string publicId, int width = 1024, int height = 1024, string crop = "fill");

    Task<PostImage> UploadAsync(UploadParameters parameters, Stream stream,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(List<string> publicIds, CancellationToken cancellationToken = default);

    Task SetAccessControlAsync(string publicId, bool isPublic, CancellationToken cancellationToken = default);
}
