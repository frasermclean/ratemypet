using System.Net;
using RateMyPet.Core;
using RateMyPet.Initializer;

namespace RateMyPet.Api.Endpoints.Posts.Comments;

[Collection(nameof(ApiCollection))]
[Trait("Category", "Integration")]
public class AddPostCommentEndpointTests(ApiFixture fixture) : TestBase<ApiFixture>
{
    [Fact]
    public async Task AddPostComment_WithValidData_ShouldReturnOk()
    {
        // arrange
        var httpClient = fixture.ContributorClient;
        const string content = "This is a test comment";
        var request = CreateRequest(SeedData.Posts[0].Id, content);

        // act
        var (message, response) =
            await httpClient.POSTAsync<AddPostCommentEndpoint, AddPostCommentRequest, PostCommentResponse>(request);

        // assert
        message.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.Id.ShouldNotBe(Guid.Empty);
        response.Content.ShouldBe(content);
        response.IsDeleted.ShouldBeFalse();
        response.AuthorUserName.ShouldBeNull();
        response.CreatedAtUtc.ShouldBeGreaterThan(DateTime.UtcNow - TimeSpan.FromSeconds(60));
        response.UpdatedAtUtc.ShouldBeNull();
        response.ParentId.ShouldBeNull();
        response.Replies.ShouldBeNull();
    }

    [Fact]
    public async Task AddPostComment_WithInvalidPostId_ShouldReturnBadRequest()
    {
        // arrange
        var httpClient = fixture.ContributorClient;
        var request = CreateRequest(content: "This post doesn't exist");

        // act
        var (message, problemDetails) =
            await httpClient.POSTAsync<AddPostCommentEndpoint, AddPostCommentRequest, ProblemDetails>(request);

        // assert
        message.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        problemDetails.Errors.ShouldHaveSingleItem();
        problemDetails.Errors.ShouldContain(error => error.Name == "postId" && error.Reason == "Invalid post ID");
    }

    [Fact]
    public async Task AddPostComment_WithTooLongContent_ShouldReturnBadRequest()
    {
        // arrange
        var httpClient = fixture.ContributorClient;
        var request = CreateRequest(content: new string('a', PostComment.ContentMaxLength + 1));

        // act
        var (message, problemDetails) =
            await httpClient.POSTAsync<AddPostCommentEndpoint, AddPostCommentRequest, ProblemDetails>(request);

        // assert
        message.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        problemDetails.Errors.ShouldHaveSingleItem();
        problemDetails.Errors.ShouldContain(error => error.Name == "content" && error.Reason ==
            $"Content must not be empty and must not exceed {PostComment.ContentMaxLength} characters.");
    }

    private static AddPostCommentRequest CreateRequest(
        Guid? postId = null,
        string content = "",
        Guid? parentId = null) => new()
    {
        PostId = postId ?? Guid.Empty,
        Content = content,
        ParentId = parentId
    };
}
