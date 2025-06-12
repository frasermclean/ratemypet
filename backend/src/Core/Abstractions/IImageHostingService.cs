using FluentResults;

namespace RateMyPet.Core.Abstractions;

public interface IImageHostingService
{
    Task<Result<PostImage>> GetAsync(string publicId, CancellationToken cancellationToken = default);

    Uri GetPublicUri(string publicId, int width = 1024, int height = 1024, string crop = "fill");

    Task<Result<PostImage>> UploadAsync(string fileName, Stream stream, Post post,
        CancellationToken cancellationToken = default);

    Task<Result> DeleteAsync(List<string> publicIds, CancellationToken cancellationToken = default);

    Task<Result> SetAccessControlAsync(string publicId, bool isPublic,
        CancellationToken cancellationToken = default);
}
