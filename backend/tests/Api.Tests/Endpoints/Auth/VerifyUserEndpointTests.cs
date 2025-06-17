using System.Net;
using RateMyPet.Api.Endpoints.Auth;

namespace RateMyPet.Api.Tests.Endpoints.Auth;

[Collection(nameof(ApiCollection))]
[Trait("Category", "Integration")]
public class VerifyUserEndpointTests(ApiFixture fixture) : TestBase<ApiFixture>
{
    [Fact]
    public async Task VerifyUser_WithAuthenticatedUser_ShouldReturnOk()
    {
        // arrange
        var httpClient = fixture.ContributorClient;

        // act
        var (message, response) = await httpClient.GETAsync<VerifyUserEndpoint, VerifyUserResponse>();

        // assert
        message.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.IsAuthenticated.ShouldBeTrue();
        response.User.ShouldNotBeNull();
    }

    [Fact]
    public async Task VerifyUser_WithUnauthenticatedUser_ShouldReturnOk()
    {
        // act
        var (message, response) = await fixture.Client.GETAsync<VerifyUserEndpoint, VerifyUserResponse>();

        // assert
        message.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.IsAuthenticated.ShouldBeFalse();
    }
}
