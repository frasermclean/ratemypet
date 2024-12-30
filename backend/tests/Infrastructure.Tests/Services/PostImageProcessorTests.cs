﻿using Microsoft.Extensions.Logging;
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
        var previewBlobName = postImageProcessor.GetBlobName(post.Id, ImageSize.Preview);
        var fullBlobName = postImageProcessor.GetBlobName(post.Id, ImageSize.Full);
        var contentType = options.ContentType;
        await using var fileStream = File.OpenRead("Data/red_1600x1200.png");
        const string previewFile = "preview.png";
        const string fullFile = "full.png";

        blobContainerManagerMock.Setup(manager =>
                manager.OpenWriteStreamAsync(previewBlobName, contentType, It.IsAny<CancellationToken>()))
            .ReturnsAsync(File.OpenWrite(previewFile));
        blobContainerManagerMock.Setup(manager =>
                manager.OpenWriteStreamAsync(fullBlobName, contentType, It.IsAny<CancellationToken>()))
            .ReturnsAsync(File.OpenWrite(fullFile));

        // act
        var result = await postImageProcessor.ProcessOriginalImageAsync(fileStream, post);
        using var previewImage = await Image.LoadAsync(previewFile);
        using var fullImage = await Image.LoadAsync(fullFile);

        // assert
        result.Should().BeSuccess();
        previewImage.Width.Should().Be(options.PreviewWidth);
        previewImage.Height.Should().Be(options.PreviewHeight);
        previewImage.Metadata.DecodedImageFormat!.DefaultMimeType.Should().Be(contentType);
        fullImage.Width.Should().Be(options.FullWidth);
        fullImage.Height.Should().Be(options.FullHeight);
        fullImage.Metadata.DecodedImageFormat!.DefaultMimeType.Should().Be(contentType);
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
        Species = new Species { Name = "Dog" }
    };
}
