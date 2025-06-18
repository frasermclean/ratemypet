using System.Net;
using RateMyPet.Api.Endpoints.Posts;
using RateMyPet.Initializer;

namespace RateMyPet.Api.Tests.Endpoints.Posts;

[Collection(nameof(ApiCollection))]
[Trait("Category", "Integration")]
public class GetPostBySlugEndpointTests(ApiFixture fixture) : TestBase<ApiFixture>
{
    [Fact]
    public async Task GetPostBySlug_WithValidSlug_ShouldReturnOk()
    {
        // arrange
        var httpClient = fixture.Client;
        var postSlug = SeedData.Posts[0].Slug;
        var request = CreateRequest(postSlug);

        // act
        var (message, response) =
            await httpClient.GETAsync<GetPostBySlugEndpoint, GetPostBySlugRequest, PostResponse>(request);

        // assert
        message.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.Slug.ShouldBe(postSlug);
    }

    [Fact]
    public async Task GetPostBySlug_WithUnknownSlug_ShouldReturnNotFound()
    {
        // arrange
        var httpClient = fixture.Client;
        const string postSlug = "unknown-slug-123";
        var request = CreateRequest(postSlug);

        // act
        var message = await httpClient.GETAsync<GetPostBySlugEndpoint, GetPostBySlugRequest>(request);

        // assert
        message.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    private static GetPostBySlugRequest CreateRequest(string postSlug) => new()
    {
        PostSlug = postSlug
    };
}
