using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace RateMyPet.Infrastructure.Services.ImageHosting;

public interface IImageHostingService
{
    Task<ImageUploadResult> UploadAsync(string fileName, Stream stream, string displayName,
        string publicId, CancellationToken cancellationToken = default);

    Task<DelResResult> DeleteAsync(List<string> publicIds, CancellationToken cancellationToken = default);
}

public class ImageHostingService : IImageHostingService
{
    private readonly ILogger<ImageHostingService> logger;
    private readonly Cloudinary cloudinary;

    public ImageHostingService(IOptions<CloudinaryOptions> options, HttpClient httpClient,
        ILogger<ImageHostingService> logger)
    {
        this.logger = logger;
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

    public async Task<ImageUploadResult> UploadAsync(string fileName, Stream stream, string displayName,
        string publicId, CancellationToken cancellationToken = default)
    {
        var parameters = new ImageUploadParams
        {
            File = new FileDescription(fileName, stream),
            DisplayName = displayName,
            PublicId = publicId,
            AssetFolder = "posts",
            UseAssetFolderAsPublicIdPrefix = true
        };

        var result = await cloudinary.UploadAsync(parameters, cancellationToken);

        logger.LogInformation("Uploaded image with public ID: {PublicId}", result.PublicId);

        return result;
    }

    public async Task<DelResResult> DeleteAsync(List<string> publicIds,
        CancellationToken cancellationToken = default)
    {
        var parameters = new DelResParams
        {
            ResourceType = ResourceType.Image,
            PublicIds = publicIds
        };

        var result = await cloudinary.DeleteResourcesAsync(parameters, cancellationToken);

        logger.LogInformation("Deleted {Count} images", result.Deleted.Count);

        return result;
    }
}
