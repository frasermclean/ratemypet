using System.Net;

namespace RateMyPet.Api.Endpoints.Auth;

[Collection(nameof(ApiCollection))]
[Trait("Category", "Integration")]
public class ForgotPasswordEndpointTests(ApiFixture fixture) : TestBase<ApiFixture>
{
    [Fact]
    public async Task ForgotPassword_WithUnknownEmailAddress_ShouldReturnNoContent()
    {
        // arrange
        var request = CreateRequest();

        // act
        var message = await fixture.Client.POSTAsync<ForgotPasswordEndpoint, ForgotPasswordRequest>(request);

        // assert
        message.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }

    private static ForgotPasswordRequest CreateRequest(string emailAddress = "bob.smith@example.com") => new()
    {
        EmailAddress = emailAddress
    };
}
