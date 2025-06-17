using System.Net;
using RateMyPet.Api.Endpoints.Posts;
using RateMyPet.Core;
using RateMyPet.Initializer;

namespace RateMyPet.Api.Tests.Endpoints.Posts;

[Collection(nameof(ApiCollection))]
[Trait("Category", "Integration")]
public class UpdatePostEndpointTests(ApiFixture fixture) : TestBase<ApiFixture>
{
    private static readonly Post ExistingPost = SeedData.Posts[2];

    [Fact]
    public async Task UpdatePost_WithValidRequest_ShouldReturnOk()
    {
        // arrange
        var httpClient = fixture.ContributorClient;
        const string newDescription = "What a good boy!";
        var tags = new List<string> { "big", "bouncy", "boy" };
        var request = CreateRequest(ExistingPost.Id, newDescription, ExistingPost.SpeciesId, tags);

        // act
        var (message, response) =
            await httpClient.PUTAsync<UpdatePostEndpoint, UpdatePostRequest, PostResponse>(request);

        // assert
        message.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.Description.ShouldBe(newDescription);
        response.Tags.ShouldBe(tags);
        response.UpdatedAt.ShouldNotBeNull();
    }

    [Fact]
    public async Task UpdatePost_WithInvalidSpeciesId_ShouldReturnBadRequest()
    {
        // arrange
        var httpClient = fixture.ContributorClient;
        var request = CreateRequest(postId: ExistingPost.Id, speciesId: -1);

        // act
        var (message, problemDetails) =
            await httpClient.PUTAsync<UpdatePostEndpoint, UpdatePostRequest, ProblemDetails>(request);

        // assert
        message.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        problemDetails.Errors.ShouldContain(error => error.Name == "speciesId" && error.Reason == "Invalid species ID");
    }

    [Fact]
    public async Task UpdatePost_WithInvalidPostId_ShouldReturnNotFound()
    {
        // arrange
        var httpClient = fixture.ContributorClient;
        var request = CreateRequest(Guid.Empty);

        // act
        var message = await httpClient.PUTAsync<UpdatePostEndpoint, UpdatePostRequest>(request);

        // assert
        message.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    private static UpdatePostRequest CreateRequest(
        Guid postId,
        string description = "New description",
        int speciesId = 1,
        List<string>? tags = null) => new()
    {
        PostId = postId,
        Description = description,
        SpeciesId = speciesId,
        Tags = tags ?? []
    };
}
