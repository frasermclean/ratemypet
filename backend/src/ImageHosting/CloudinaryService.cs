using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using FluentResults;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RateMyPet.Core;

namespace RateMyPet.ImageHosting;

public class CloudinaryService : IImageHostingService
{
    private readonly ILogger<CloudinaryService> logger;
    private readonly string environment;
    private readonly Cloudinary cloudinary;

    public CloudinaryService(IOptions<CloudinaryOptions> options, HttpClient httpClient,
        ILogger<CloudinaryService> logger, IHostEnvironment environment)
    {
        this.logger = logger;
        this.environment = environment.EnvironmentName.ToLower();

        var (cloudName, apiKey, apiSecret) = options.Value;
        var account = new Account(cloudName, apiKey, apiSecret);

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

    public Uri GetPublicUri(string publicId, int width, int height, string crop)
    {
        var transformation = new Transformation()
            .Width(width)
            .Height(height)
            .Crop(crop);

        var url = cloudinary.Api.UrlImgUp
            .Transform(transformation)
            .BuildUrl(publicId);

        return new Uri(url);
    }

    public async Task<PostImage> UploadAsync(UploadParameters parameters, Stream stream,
        CancellationToken cancellationToken = default)
    {
        var result = await cloudinary.UploadAsync(new ImageUploadParams
        {
            File = new FileDescription(parameters.FileName, stream),
            DisplayName = parameters.Title,
            PublicId = parameters.Slug,
            AssetFolder = parameters.Environment.Equals("prod", StringComparison.InvariantCultureIgnoreCase)
                ? "posts"
                : $"{environment}/posts",
            UseAssetFolderAsPublicIdPrefix = true,
            Context = new StringDictionary($"caption={parameters.Description}", $"alt={parameters.Title}"),
            MetadataFields = new StringDictionary(
                $"environment={parameters.Environment.ToLowerInvariant()}",
                $"species_id={parameters.SpeciesId}",
                $"user_id={parameters.UserId}"
            )
        }, cancellationToken);

        if (result.Error is not null)
        {
            logger.LogError("Failed to upload image: {Error}", result.Error.Message);
            throw new CloudinaryServerException(result.Error);
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

    public async Task<Result> SetAccessControlAsync(string publicId, bool isPublic,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(publicId);

        var parameters = new UpdateParams(publicId)
        {
            AccessControl =
            [
                new AccessControlRule { AccessType = isPublic ? AccessType.Anonymous : AccessType.Token }
            ]
        };

        var result = await cloudinary.UpdateResourceAsync(parameters, cancellationToken);

        if (result.Error is not null)
        {
            logger.LogError("Failed to set public access for images: {Error}", result.Error.Message);
            return Result.Fail(result.Error.Message);
        }

        logger.LogInformation("Set {AccessType} access for image with public ID: {PublicId}",
            isPublic ? "public" : "restricted", publicId);

        return Result.Ok();
    }
}
