using Microsoft.AspNetCore.Http;
using RateMyPet.Core;
using RateMyPet.Infrastructure;

namespace RateMyPet.Api.Extensions;

public class PostExtensionsTests
{
    private readonly Fixture fixture = new();

    [Fact]
    public void GetImageUrl_WithNullRequest_ReturnsNull()
    {
        // arrange
        var post = fixture.Create<Post>();

        // act
        var uri = post.GetImageUrl(null);

        // assert
        uri.Should().BeNull();
    }

    [Theory]
    [InlineData("localhost:5443", 5443)]
    [InlineData("api.ratemy.pet", 443)]
    public void GetImageUrl_WithRequest_ReturnsUri(string host, int expectedPort)
    {
        // arrange
        var post = fixture.Create<Post>();
        var requestMock = new Mock<HttpRequest>();
        requestMock.SetupGet(request => request.Host).Returns(new HostString(host));

        // act
        var uri = post.GetImageUrl(requestMock.Object);

        // assert
        uri.Should().NotBeNull();
        uri!.ToString().Should().Contain(host);
        uri.Scheme.Should().Be("https");
        uri.AbsolutePath.Should().Be($"/{BlobContainerNames.Images}/{post.Id}");
        uri.Port.Should().Be(expectedPort);
    }
}
