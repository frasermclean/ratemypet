using System.Net;
using RateMyPet.Api.Endpoints.Posts.Comments;
using RateMyPet.Initializer;

namespace RateMyPet.Api.Tests.Endpoints.Posts.Comments;

[Collection(nameof(ApiCollection))]
[Trait("Category", "Integration")]
public class DeletePostCommentEndpointTests(ApiFixture fixture) : TestBase<ApiFixture>
{
    private static readonly Guid PostId = SeedData.Posts[0].Id;
    private static readonly Guid CommentId = SeedData.Posts[0].Comments[2].Id;

    [Fact]
    public async Task DeletePostComment_WithValidData_ShouldReturnNoContent()
    {
        // arrange
        var httpClient = fixture.ContributorClient;
        var request = CreateRequest(PostId, CommentId);

        // act
        var message = await httpClient.DELETEAsync<DeletePostCommentEndpoint, DeletePostCommentRequest>(request);

        // assert
        message.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeletePostComment_WithInvalidPostId_ShouldReturnNotFound()
    {
        // arrange
        var httpClient = fixture.ContributorClient;
        var request = CreateRequest(Guid.Empty, SeedData.Posts[0].Comments[2].Id);

        // act
        var message = await httpClient.DELETEAsync<DeletePostCommentEndpoint, DeletePostCommentRequest>(request);

        // assert
        message.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeletePostComment_WithInvalidCommentId_ShouldReturnNotFound()
    {
        // arrange
        var httpClient = fixture.ContributorClient;
        var request = CreateRequest(PostId, Guid.Empty);

        // act
        var message = await httpClient.DELETEAsync<DeletePostCommentEndpoint, DeletePostCommentRequest>(request);

        // assert
        message.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeletePostComment_WithUnauthorizedUser_ShouldReturnForbidden()
    {
        // arrange
        var httpClient = fixture.ContributorClient;
        var commentId = SeedData.Posts[0].Comments[1].Id;
        var request = CreateRequest(PostId, commentId);

        // act
        var message = await httpClient.DELETEAsync<DeletePostCommentEndpoint, DeletePostCommentRequest>(request);

        // assert
        message.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    private static DeletePostCommentRequest CreateRequest(Guid postId = default, Guid commentId = default) => new()
    {
        PostId = postId,
        CommentId = commentId
    };
}
