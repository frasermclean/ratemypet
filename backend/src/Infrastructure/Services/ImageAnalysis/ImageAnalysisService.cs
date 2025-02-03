using Azure.AI.Vision.ImageAnalysis;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RateMyPet.Core.Abstractions;

namespace RateMyPet.Infrastructure.Services.ImageAnalysis;

public class ImageAnalysisService(
    IOptions<CognitiveServicesOptions> options,
    ILogger<ImageAnalysisService> logger,
    ImageAnalysisClient client) : IImageAnalysisService
{
    private readonly float tagConfidenceThreshold = options.Value.TagConfidenceThreshold;

    public async Task<IEnumerable<string>> AnalyzeTagsAsync(Uri imageUri, CancellationToken cancellationToken = default)
    {
        var response = await client.AnalyzeAsync(imageUri, VisualFeatures.Tags, cancellationToken: cancellationToken);

        // select tags with single word names and high confidence
        var tags = response.Value.Tags.Values
            .Where(tag => !tag.Name.Contains(' ') && tag.Confidence > tagConfidenceThreshold)
            .Select(tag => tag.Name);

        logger.LogInformation("Image {ImageUri} analyzed. Tags: {Tags}", imageUri, tags);

        return tags;
    }
}
