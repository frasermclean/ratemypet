using Microsoft.Extensions.Logging;
using RateMyPet.Core;
using RateMyPet.Core.Abstractions;
using RateMyPet.Infrastructure.Errors;
using RateMyPet.Infrastructure.Options;
using RateMyPet.Infrastructure.Services;
using SixLabors.ImageSharp;
using MicrosoftOptions = Microsoft.Extensions.Options.Options;

namespace RateMyPet.Infrastructure.Tests.Services;

public class PostImageProcessorTests
{
    private readonly Mock<IBlobContainerManager> blobContainerManagerMock = new();
    private readonly Mock<ILogger<PostImageProcessor>> loggerMock = new();

    private readonly ImageProcessingOptions options = new()
    {
        ContentType = "image/png",
        PreviewWidth = 320,
        PreviewHeight = 320,
        FullWidth = 1024,
        FullHeight = 1024
    };

    private readonly PostImageProcessor postImageProcessor;

    public PostImageProcessorTests()
    {
        postImageProcessor = new PostImageProcessor(MicrosoftOptions.Create(options), loggerMock.Object,
            blobContainerManagerMock.Object);
    }

    [Fact]
    public async Task ProcessOriginalImage_WithValidImage_ShouldReturnSuccess()
    {
        // arrange
        var post = CreatePost();
        var blobName = postImageProcessor.GetBlobName(post.Id);
        var contentType = options.ContentType;
        await using var fileStream = File.OpenRead("Data/red_1600x1200.png");
        const string outputFile = "output.png";

        blobContainerManagerMock.Setup(manager =>
                manager.OpenWriteStreamAsync(blobName, contentType, It.IsAny<CancellationToken>()))
            .ReturnsAsync(File.OpenWrite(outputFile));

        // act
        var result = await postImageProcessor.ProcessOriginalImageAsync(fileStream, post);
        using var outputImage = await Image.LoadAsync(outputFile);

        // assert
        result.Should().BeSuccess();
        outputImage.Width.Should().Be(options.PreviewWidth);
        outputImage.Height.Should().Be(options.PreviewHeight);
        outputImage.Metadata.DecodedImageFormat!.DefaultMimeType.Should().Be(contentType);
    }

    [Fact]
    public async Task ProcessOriginalImage_WithTooSmallImage_ShouldReturnError()
    {
        // arrange
        var post = CreatePost();
        await using var fileStream = File.OpenRead("Data/green_256x256.png");

        // act
        var result = await postImageProcessor.ProcessOriginalImageAsync(fileStream, post);

        // assert
        result.Should().BeFailure().And
            .HaveReason<ImageTooSmallError>("Image dimensions are too small, dimensions: 256x256");
    }

    [Fact]
    public async Task ProcessOriginalImage_WithInvalidImage_ShouldReturnError()
    {
        // arrange
        var post = CreatePost();
        await using var fileStream = File.OpenRead("Data/random.bin");

        // act
        var result = await postImageProcessor.ProcessOriginalImageAsync(fileStream, post);

        // assert
        result.Should().BeFailure().And
            .HaveReason<ImageProcessingError>("Unknown image format");
    }

    private static Post CreatePost() => new()
    {
        Title = "My first post",
        User = new User(),
        Species = new Species { Name = "Dog" },
        Image = new PostImage
        {
            BlobName = "original",
            FileName = "image1.png",
            MimeType = "image/png",
            Width = 1024,
            Height = 768
        }
    };
}
