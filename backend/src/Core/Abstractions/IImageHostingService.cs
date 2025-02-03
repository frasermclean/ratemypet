using FluentResults;

namespace RateMyPet.Core.Abstractions;

public interface IImageHostingService
{
    Task<Result<PostImage>> GetAsync(string publicId, CancellationToken cancellationToken = default);

    Task<Uri> GetPublicUrl(string publicId, CancellationToken cancellationToken = default);

    Task<Result<PostImage>> UploadAsync(string fileName, Stream stream, Post post,
        CancellationToken cancellationToken = default);

    Task<Result> DeleteAsync(List<string> publicIds, CancellationToken cancellationToken = default);
}
