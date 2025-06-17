using System.Net;
using RateMyPet.Initializer;

namespace RateMyPet.Api.Tests.Endpoints.Posts;

[Collection(nameof(ApiCollection))]
[Trait("Category", "Integration")]
public class GetPostEndpointTests(ApiFixture fixture) : TestBase<ApiFixture>
{
    [Fact]
    public async Task GetPost_WithValidId_ShouldReturnOk()
    {
        // arrange
        var httpClient = fixture.Client;
        var postId = SeedData.Posts[0].Id;

        // act
        var message = await httpClient.GetAsync($"/api/posts/{postId}", TestContext.Current.CancellationToken);

        // assert
        message.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetPost_WithUnknownId_ShouldReturnNotFound()
    {
        // arrange
        var httpClient = fixture.Client;
        var postId = Guid.Empty;

        // act
        var message = await httpClient.GetAsync($"/api/posts/{postId}", TestContext.Current.CancellationToken);

        // assert
        message.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetPost_WithUnknownSlug_ShouldReturnNotFound()
    {
        // arrange
        var httpClient = fixture.Client;
        const string postSlug = "unknown-slug";

        // act
        var message = await httpClient.GetAsync($"/api/posts/{postSlug}", TestContext.Current.CancellationToken);

        // assert
        message.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
