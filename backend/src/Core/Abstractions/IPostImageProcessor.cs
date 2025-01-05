using FluentResults;

namespace RateMyPet.Core.Abstractions;

public interface IPostImageProcessor
{
    string GetBlobName(Guid postId);

    Task<Result<(int Width, int Height)>> ValidateImageAsync(Stream stream,
        CancellationToken cancellationToken = default);

    Task<Result<string>> ProcessOriginalImageAsync(Stream stream, Post post,
        CancellationToken cancellationToken = default);
}
