﻿using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using FluentResults;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RateMyPet.Core;
using RateMyPet.Core.Abstractions;

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
