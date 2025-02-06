using Azure.AI.ContentSafety;
using Azure.AI.Vision.ImageAnalysis;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RateMyPet.Core.Abstractions;
using RateMyPet.Core.Results;

namespace RateMyPet.Infrastructure.Services.ImageAnalysis;

public class ImageAnalysisService(
    IOptions<ImageAnalysisOptions> options,
    ILogger<ImageAnalysisService> logger,
    ImageAnalysisClient analysisClient,
    ContentSafetyClient safetyClient,
    HttpClient httpClient) : IImageAnalysisService
{
    private readonly float tagConfidenceThreshold = options.Value.TagConfidenceThreshold;
    private readonly int safetyCategoryThreshold = options.Value.SafetyCategoryThreshold;

    public async Task<IEnumerable<string>> AnalyzeTagsAsync(Uri imageUri, CancellationToken cancellationToken = default)
    {
        var response =
            await analysisClient.AnalyzeAsync(imageUri, VisualFeatures.Tags, cancellationToken: cancellationToken);

        // select tags with single word names and high confidence
        var tags = response.Value.Tags.Values
            .Where(tag => !tag.Name.Contains(' ') && tag.Confidence > tagConfidenceThreshold)
            .Select(tag => tag.Name);

        logger.LogInformation("Image {ImageUri} analyzed. Tags: {Tags}", imageUri, tags);

        return tags;
    }

    public async Task<ImageSafetyResult> AnalyzeSafetyAsync(Uri imageUri, CancellationToken cancellationToken)
    {
        var imageBytes = await httpClient.GetByteArrayAsync(imageUri, cancellationToken);
        var binaryData = new BinaryData(imageBytes);

        var response = await safetyClient.AnalyzeImageAsync(binaryData, cancellationToken);

        var categories = response.Value.CategoriesAnalysis
            .ToDictionary(analysis => analysis.Category, analysis => analysis.Severity);

        logger.LogInformation("Image {ImageUri} analyzed. Severities: {Severities}", imageUri, categories);

        return new ImageSafetyResult
        {
            IsHate = categories[ImageCategory.Hate] > safetyCategoryThreshold,
            IsSelfHarm = categories[ImageCategory.SelfHarm] > safetyCategoryThreshold,
            IsSexual = categories[ImageCategory.Sexual] > safetyCategoryThreshold,
            IsViolence = categories[ImageCategory.Violence] > safetyCategoryThreshold
        };
    }
}
