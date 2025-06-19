using System.Net;
using RateMyPet.Api.Endpoints.Posts;
using RateMyPet.Initializer;

namespace RateMyPet.Api.Tests.Endpoints.Posts;

[Collection(nameof(ApiCollection))]
[Trait("Category", "Integration")]
public class GetPostByIdEndpointTests(ApiFixture fixture) : TestBase<ApiFixture>
{
    [Fact]
    public async Task GetPostById_WithValidId_ShouldReturnOk()
    {
        // arrange
        var httpClient = fixture.Client;
        var postId = SeedData.Posts[0].Id;
        var request = CreateRequest(postId);

        // act
        var (message, response) =
            await httpClient.GETAsync<GetPostByIdEndpoint, GetPostByIdRequest, PostResponse>(request);

        // assert
        message.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.Id.ShouldBe(postId);
    }

    [Fact]
    public async Task GetPostById_WithUnknownId_ShouldReturnNotFound()
    {
        // arrange
        var httpClient = fixture.Client;
        var request = CreateRequest();

        // act
        var message = await httpClient.GETAsync<GetPostByIdEndpoint, GetPostByIdRequest>(request);

        // assert
        message.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    private static GetPostByIdRequest CreateRequest(Guid postId = default) => new()
    {
        PostId = postId
    };
}
