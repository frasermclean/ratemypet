using System.Net;

namespace RateMyPet.Api.Endpoints.Auth;

[Collection(nameof(ApiCollection))]
[Trait("Category", "Integration")]
public class VerifyUserEndpointTests(ApiFixture fixture) : TestBase<ApiFixture>
{
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
