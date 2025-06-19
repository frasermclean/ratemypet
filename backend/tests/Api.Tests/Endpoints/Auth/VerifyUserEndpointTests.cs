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
        response.User.UserId.ShouldNotBe(Guid.Empty);
        response.User.UserName.ShouldNotBeNullOrEmpty();
        response.User.EmailAddress.ShouldNotBeNullOrEmpty();
        response.User.EmailHash.ShouldNotBeNullOrEmpty();
        response.User.Roles.ShouldNotBeEmpty();
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
