using System.Net;
using FastEndpoints;
using FastEndpoints.Testing;
using Gridify;

namespace RateMyPet.Api.Endpoints.Posts;

[Collection(nameof(ApiCollection))]
[Trait("Category", "Integration")]
public class SearchPostsEndpointTests(ApiFixture fixture) : TestBase<ApiFixture>
{
    [Fact]
    public async Task SearchPosts_WithValidQuery_ShouldReturnOk()
    {
        // arrange
        var query = new GridifyQuery();

        // act
        var (message, paging) = await fixture.Client.GETAsync<SearchPostsEndpoint, GridifyQuery, Paging<SearchPostsMatch>>(query);

        // assert
        message.StatusCode.ShouldBe(HttpStatusCode.OK);
        message.Headers.ETag.ShouldNotBeNull();
    }

}
