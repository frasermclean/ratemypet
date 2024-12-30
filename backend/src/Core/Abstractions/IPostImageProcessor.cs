using FluentResults;

namespace RateMyPet.Core.Abstractions;

public interface IPostImageProcessor
{
    string GetBlobName(Guid postId, ImageSize size);

    Task<Result<PostImage>> ProcessOriginalImageAsync(Stream stream, Post post,
        CancellationToken cancellationToken = default);
}
