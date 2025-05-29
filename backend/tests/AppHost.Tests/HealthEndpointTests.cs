using System.Net;

namespace RateMyPet.AppHost.Tests;

public class HealthEndpointTests(AppHostFixture fixture)
{
    [Fact]
    public async Task GetHealth_Should_ReturnOk()
    {
        // arrange
        var httpClient = fixture.CreateHttpClient("api");

        // act
        var response = await httpClient.GetAsync("/health", TestContext.Current.CancellationToken);

        // assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}
