using System.Net;
using Gridify;
using RateMyPet.Api.Endpoints.Posts;

namespace RateMyPet.Api.Tests.Endpoints.Posts;

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
        var (message, paging) =
            await fixture.Client.GETAsync<SearchPostsEndpoint, GridifyQuery, Paging<SearchPostsMatch>>(query);

        // assert
        message.StatusCode.ShouldBe(HttpStatusCode.OK);
        message.Headers.ETag.ShouldNotBeNull();
        paging.Count.ShouldBeGreaterThan(0);
    }
}
