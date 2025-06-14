using System.Net;
using RateMyPet.Core;

namespace RateMyPet.Api.Endpoints.Auth;

[Collection(nameof(ApiCollection))]
[Trait("Category", "Integration")]
public class LoginEndpointTests(ApiFixture fixture) : TestBase<ApiFixture>
{
    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturnOk()
    {
        // arrange
        var request = new LoginRequest
        {
            EmailOrUserName = ApiFixture.AdministratorUserName,
            Password = ApiFixture.AdministratorPassword
        };

        // act
        var (message, response) = await fixture.Client.POSTAsync<LoginEndpoint, LoginRequest, LoginResponse>(request);

        // assert
        message.StatusCode.ShouldBe(HttpStatusCode.OK);
        message.Headers.ShouldContain(pair => pair.Key == "Set-Cookie" && pair.Value.Any(value => value.StartsWith("RateMyPet.Auth")));
        response.UserId.ShouldNotBe(Guid.Empty);
        response.UserName.ShouldBe(ApiFixture.AdministratorUserName);
        response.EmailAddress.ShouldBe(ApiFixture.AdministratorEmail);
        response.EmailHash.ShouldNotBeNullOrEmpty();
        response.Roles.ShouldContain(Role.Contributor);
        response.Roles.ShouldContain(Role.Administrator);
    }

    [Fact]
    public async Task Login_WithInvalidUsername_ShouldReturnUnauthorized()
    {
        // arrange
        var request = new LoginRequest
        {
            EmailOrUserName = "InvalidUserName",
            Password = ApiFixture.AdministratorPassword
        };

        // act
        var (message, _) = await fixture.Client.POSTAsync<LoginEndpoint, LoginRequest, LoginResponse>(request);

        // assert
        message.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_WithInvalidPassword_ShouldReturnUnauthorized()
    {
        // arrange
        var request = new LoginRequest
        {
            EmailOrUserName = ApiFixture.AdministratorUserName,
            Password = "InvalidPassword123!"
        };

        // act
        var (message, _) = await fixture.Client.POSTAsync<LoginEndpoint, LoginRequest, LoginResponse>(request);

        // assert
        message.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}
