using System.Net;
using RateMyPet.Api.Endpoints.Posts.Reactions;
using RateMyPet.Core;
using RateMyPet.Initializer;

namespace RateMyPet.Api.Tests.Endpoints.Posts.Reactions;

[Collection(nameof(ApiCollection))]
[Trait("Category", "Integration")]
public class AddPostReactionEndpointTests(ApiFixture fixture) : TestBase<ApiFixture>
{
    [Fact]
    public async Task AddPostReaction_WithNoPreviousReaction_ShouldReturnNoContent()
    {
        // arrange
        var httpClient = fixture.ContributorClient;
        var postId = SeedData.Posts[2].Id;
        var request = CreateRequest(postId);

        // act
        var message = await httpClient.POSTAsync<AddPostReactionEndpoint, AddPostReactionRequest>(request);

        // assert
        message.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task AddPostReaction_WithExistingReaction_ShouldReturnBadRequest()
    {
        // arrange
        var httpClient = fixture.ContributorClient;
        var postId = SeedData.Posts[0].Id;
        var request = CreateRequest(postId, Reaction.Funny);

        // act
        var (message, problemDetails) =
            await httpClient.POSTAsync<AddPostReactionEndpoint, AddPostReactionRequest, ProblemDetails>(request);

        // assert
        message.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        problemDetails.Errors.ShouldHaveSingleItem()
            .ShouldSatisfyAllConditions(error =>
            {
                error.Name.ShouldBe("reaction");
                error.Reason.ShouldBe("You have already reacted to this post.");
            });
    }

    [Fact]
    public async Task AddPostReaction_WithAnonymousUser_ShouldReturnUnauthorized()
    {
        // arrange
        var httpClient = fixture.Client;
        var request = CreateRequest();

        // act
        var message = await httpClient.POSTAsync<AddPostReactionEndpoint, AddPostReactionRequest>(request);

        // assert
        message.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    private static AddPostReactionRequest CreateRequest(
        Guid postId = default,
        Reaction reaction = Reaction.Like) => new()
    {
        PostId = postId,
        Reaction = reaction
    };
}
