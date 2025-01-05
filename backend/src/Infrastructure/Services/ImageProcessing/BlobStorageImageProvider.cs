using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp.Web.Providers;
using SixLabors.ImageSharp.Web.Resolvers;

namespace RateMyPet.Infrastructure.Services.ImageProcessing;

public class BlobStorageImageProvider(
    BlobServiceClient blobServiceClient,
    IOptions<ImageProcessingOptions> options,
    FormatUtilities formatUtilities)
    : IImageProvider
{
    /// <summary>
    /// Character array to remove from paths.
    /// </summary>
    private static readonly char[] SlashChars = ['\\', '/'];

    private readonly string containerName = options.Value.ImagesContainerName;
    private readonly BlobContainerClient containerClient =
        blobServiceClient.GetBlobContainerClient(options.Value.ImagesContainerName);

    private Func<HttpContext, bool>? match;

    public bool IsValidRequest(HttpContext context) =>
        formatUtilities.TryGetExtensionFromUri(context.Request.GetDisplayUrl(), out _);

    public async Task<IImageResolver?> GetAsync(HttpContext context)
    {
        // remove leading slash
        var path = context.Request.Path.Value?.TrimStart(SlashChars);
        if (path is null)
        {
            return null;
        }

        var index = path.IndexOfAny(SlashChars);
        var nameToMatch = index != -1 ? path[..index] : path;

        // ensure the container name matches
        if (!nameToMatch.Equals(containerName, StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        // blob name should be the remaining path string
        var blobName = path[containerName.Length..].TrimStart(SlashChars);
        if (string.IsNullOrWhiteSpace(blobName))
        {
            return null;
        }

        // ensure the blob exists
        var blobClient = containerClient.GetBlobClient(blobName);
        if (!await blobClient.ExistsAsync())
        {
            return null;
        }

        return new BlobStorageImageResolver(blobClient);
    }

    public ProcessingBehavior ProcessingBehavior { get; } = ProcessingBehavior.All;

    public Func<HttpContext, bool> Match
    {
        get => match ?? IsMatch;
        set => match = value;
    }

    private bool IsMatch(HttpContext context)
    {
        var path = context.Request.Path.Value?.TrimStart(SlashChars);
        return path is not null && path.StartsWith(containerName, StringComparison.OrdinalIgnoreCase);
    }
}
