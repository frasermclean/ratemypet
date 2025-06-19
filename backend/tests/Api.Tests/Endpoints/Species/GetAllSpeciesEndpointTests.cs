using System.Net;
using RateMyPet.Api.Endpoints.Species;

namespace RateMyPet.Api.Tests.Endpoints.Species;

[Collection(nameof(ApiCollection))]
[Trait("Category", "Integration")]
public class GetAllSpeciesEndpointTests(ApiFixture fixture) : TestBase<ApiFixture>
{
    [Fact]
    public async Task GetAllSpecies_ShouldReturnAllSpecies()
    {
        // act
        var (message, responses) = await fixture.Client.GETAsync<GetAllSpeciesEndpoint, IEnumerable<SpeciesResponse>>();

        // assert
        message.StatusCode.ShouldBe(HttpStatusCode.OK);
        responses.ShouldNotBeEmpty();
    }
}
