using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using FluentResults;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RateMyPet.Core;

namespace RateMyPet.Infrastructure.Services.ImageHosting;

public interface IImageHostingService
{
    Task<Result<PostImage>> GetAsync(string publicId, CancellationToken cancellationToken = default);

    Task<Result<PostImage>> UploadAsync(string fileName, Stream stream, Post post,
        CancellationToken cancellationToken = default);

    Task<Result> DeleteAsync(List<string> publicIds, CancellationToken cancellationToken = default);
}

public class ImageHostingService : IImageHostingService
{
    private readonly ILogger<ImageHostingService> logger;
    private readonly string environment;
    private readonly Cloudinary cloudinary;

    public ImageHostingService(IOptions<CloudinaryOptions> options, HttpClient httpClient,
        ILogger<ImageHostingService> logger, IHostEnvironment environment)
    {
        this.logger = logger;
        this.environment = environment.EnvironmentName.ToLower();
        var account = new Account(options.Value.CloudName, options.Value.ApiKey, options.Value.ApiSecret);

        cloudinary = new Cloudinary(account)
        {
            Api =
            {
                Secure = true,
                Client = httpClient
            }
        };
    }

    public async Task<Result<PostImage>> GetAsync(string publicId, CancellationToken cancellationToken = default)
    {
        var parameters = new GetResourceParams(publicId)
        {
            ResourceType = ResourceType.Image
        };

        var result = await cloudinary.GetResourceAsync(parameters, cancellationToken);

        if (result.Error is not null)
        {
            logger.LogError("Failed to get image: {Error}", result.Error.Message);
            return Result.Fail(result.Error.Message);
        }

        return new PostImage
        {
            AssetId = result.AssetId,
            PublicId = result.PublicId,
            Width = result.Width,
            Height = result.Height,
            Size = result.Bytes
        };
    }

    public async Task<Result<PostImage>> UploadAsync(string fileName, Stream stream, Post post,
        CancellationToken cancellationToken = default)
    {
        var parameters = new ImageUploadParams
        {
            File = new FileDescription(fileName, stream),
            DisplayName = post.Title,
            PublicId = post.Slug,
            AssetFolder = $"{environment}/{post.User.UserName}",
            UseAssetFolderAsPublicIdPrefix = true,
            Context = new StringDictionary($"caption={post.Description}", $"alt={post.Title}"),
            MetadataFields = new StringDictionary(
                $"environment={environment}",
                $"speciesName={post.Species.Name}",
                $"userName={post.User.UserName}"
            )
        };

        var result = await cloudinary.UploadAsync(parameters, cancellationToken);

        if (result.Error is not null)
        {
            logger.LogError("Failed to upload image: {Error}", result.Error.Message);
            return Result.Fail(result.Error.Message);
        }

        logger.LogInformation("Uploaded image with public ID: {PublicId}", result.PublicId);

        return new PostImage
        {
            AssetId = result.AssetId,
            PublicId = result.PublicId,
            Width = result.Width,
            Height = result.Height,
            Size = result.Bytes
        };
    }

    public async Task<Result> DeleteAsync(List<string> publicIds,
        CancellationToken cancellationToken = default)
    {
        var parameters = new DelResParams
        {
            ResourceType = ResourceType.Image,
            PublicIds = publicIds
        };

        var result = await cloudinary.DeleteResourcesAsync(parameters, cancellationToken);

        if (result.Error is not null)
        {
            logger.LogError("Failed to delete images: {Error}", result.Error.Message);
            return Result.Fail(result.Error.Message);
        }

        logger.LogInformation("Requested to deleted {Count} images", result.Deleted.Count);

        return Result.Ok();
    }
}
