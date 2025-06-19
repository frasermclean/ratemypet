using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RateMyPet.Core;

namespace RateMyPet.ImageHosting;

public class CloudinaryService : IImageHostingService
{
    private readonly ILogger<CloudinaryService> logger;
    private readonly Cloudinary cloudinary;

    public CloudinaryService(IOptions<CloudinaryOptions> options, HttpClient httpClient,
        ILogger<CloudinaryService> logger)
    {
        this.logger = logger;

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

    public async Task<PostImage> GetAsync(string publicId, CancellationToken cancellationToken = default)
    {
        var parameters = new GetResourceParams(publicId)
        {
            ResourceType = ResourceType.Image
        };

        var result = await cloudinary.GetResourceAsync(parameters, cancellationToken);

        if (result.Error is not null)
        {
            logger.LogError("Failed to get image: {Error}", result.Error.Message);
            throw new CloudinaryServerException(result.Error);
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
                : $"{parameters.Environment.ToLowerInvariant()}/posts",
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

    public async Task DeleteAsync(List<string> publicIds, CancellationToken cancellationToken = default)
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
            throw new CloudinaryServerException(result.Error);
        }

        logger.LogInformation("Requested to deleted {Count} images", result.Deleted.Count);
    }

    public async Task SetAccessControlAsync(string publicId, bool isPublic,
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
            throw new CloudinaryServerException(result.Error);
        }

        logger.LogInformation("Set {AccessType} access for image with public ID: {PublicId}",
            isPublic ? "public" : "restricted", publicId);
    }
}
