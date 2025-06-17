using System.Net;
using RateMyPet.Api.Endpoints.Posts;
using RateMyPet.Initializer;

namespace RateMyPet.Api.Tests.Endpoints.Posts;

[Collection(nameof(ApiCollection))]
[Trait("Category", "Integration")]
public class DeletePostEndpointTests(ApiFixture fixture) : TestBase<ApiFixture>
{
    [Fact]
    public async Task DeletePost_WithSoftDelete_ShouldReturnNoContent()
    {
        // arrange
        var httpClient = fixture.ContributorClient;
        var request = CreateRequest(SeedData.Posts[1].Id);

        // act
        var message = await httpClient.DELETEAsync<DeletePostEndpoint, DeletePostRequest>(request);

        // assert
        message.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeletePost_WithInvalidPostId_ShouldReturnNotFound()
    {
        // arrange
        var httpClient = fixture.ContributorClient;
        var request = CreateRequest();

        // act
        var message = await httpClient.DELETEAsync<DeletePostEndpoint, DeletePostRequest>(request);

        // assert
        message.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    private static DeletePostRequest CreateRequest(Guid postId = default, bool? shouldHardDelete = null) => new()
    {
        PostId = postId,
        ShouldHardDelete = shouldHardDelete,
    };
}
