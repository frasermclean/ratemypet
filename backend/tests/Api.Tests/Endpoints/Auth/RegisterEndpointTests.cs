﻿using System.Net;
using System.Text.Json;
using Azure.Storage.Queues;
using Microsoft.Extensions.DependencyInjection;
using RateMyPet.Api.Endpoints.Auth;
using RateMyPet.Storage;
using RateMyPet.Storage.Messaging;

namespace RateMyPet.Api.Tests.Endpoints.Auth;

[Collection(nameof(ApiCollection))]
[Trait("Category", "Integration")]
public class RegisterEndpointTests(ApiFixture fixture) : TestBase<ApiFixture>
{
    [Fact]
    public async Task Register_WithValidRequest_ShouldReturnNoContent()
    {
        // arrange
        var request = CreateRequest();

        // act
        var httpMessage = await fixture.Client.POSTAsync<RegisterEndpoint, RegisterRequest>(request);

        // assert
        httpMessage.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        var queueClient = fixture.Services.GetRequiredService<QueueServiceClient>()
            .GetQueueClient(QueueNames.RegisterConfirmation);
        var messageResponse =
            await queueClient.ReceiveMessageAsync(cancellationToken: TestContext.Current.CancellationToken);
        messageResponse.HasValue.ShouldBeTrue();
        var message = JsonSerializer.Deserialize<RegisterConfirmationMessage>(messageResponse.Value.MessageText,
            JsonSerializerOptions.Web);
        message.ShouldNotBeNull();
        message.EmailAddress.ShouldBe(request.EmailAddress);
        message.UserId.ShouldNotBe(Guid.Empty);
        message.ConfirmationToken.ShouldNotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Register_WithInvalidPassword_ShouldReturnProblemDetails()
    {
        // arrange
        var request = CreateRequest(password: "hunter2");

        // act
        var (httpMessage, problemDetails) =
            await fixture.Client.POSTAsync<RegisterEndpoint, RegisterRequest, ProblemDetails>(request);

        // assert
        httpMessage.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        httpMessage.Content.Headers.ContentType.ShouldNotBeNull().MediaType.ShouldBe("application/problem+json");
        problemDetails.Errors.ShouldContain(error => error.Name == "passwordTooShort");
        problemDetails.Errors.ShouldContain(error => error.Name == "passwordRequiresNonAlphanumeric");
        problemDetails.Errors.ShouldContain(error => error.Name == "passwordRequiresUpper");
    }

    [Fact]
    public async Task Register_WithExistingUserName_ShouldReturnProblemDetails()
    {
        // arrange
        var request = CreateRequest("frasermclean");

        // act
        var (httpMessage, problemDetails) =
            await fixture.Client.POSTAsync<RegisterEndpoint, RegisterRequest, ProblemDetails>(request);

        // assert
        httpMessage.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        httpMessage.Content.Headers.ContentType.ShouldNotBeNull().MediaType.ShouldBe("application/problem+json");
        problemDetails.Errors.ShouldContain(error => error.Name == "duplicateUserName");
    }

    private static RegisterRequest CreateRequest(
        string userName = "jane",
        string emailAddress = "jane.smith@example.com",
        string password = "Password123!") => new()
    {
        UserName = userName,
        EmailAddress = emailAddress,
        Password = password
    };
}
