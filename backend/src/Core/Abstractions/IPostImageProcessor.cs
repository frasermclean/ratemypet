using FluentResults;

namespace RateMyPet.Core.Abstractions;

public interface IPostImageProcessor
{
    Task<Result> ProcessOriginalImageAsync(Stream stream, Post post, CancellationToken cancellationToken = default);
}
