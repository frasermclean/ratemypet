﻿using System.Net;
using Microsoft.AspNetCore.Http;
using RateMyPet.Api.Endpoints.Posts;

namespace RateMyPet.Api.Tests.Endpoints.Posts;

[Collection(nameof(ApiCollection))]
[Trait("Category", "Integration")]
public class AddPostEndpointTests(ApiFixture fixture) : TestBase<ApiFixture>
{
    [Fact]
    public async Task AddPost_WithValidData_ShouldReturnCreated()
    {
        // arrange
        var httpClient = fixture.ContributorClient;
        var request = CreateRequest("New Post");

        // act
        var message = await httpClient.POSTAsync<AddPostEndpoint, AddPostRequest>(request, true);

        // assert
        message.StatusCode.ShouldBe(HttpStatusCode.Created);
        message.Headers.Location.ShouldNotBeNull().OriginalString.ShouldStartWith("/api/posts/new-post-");
    }

    [Fact]
    public async Task AddPost_WithInvalidSpeciesId_ShouldReturnBadRequest()
    {
        // arrange
        var httpClient = fixture.ContributorClient;
        var request = CreateRequest(title: "Invalid post", speciesId: -1);

        // act
        var (message, problemDetails) =
            await httpClient.POSTAsync<AddPostEndpoint, AddPostRequest, ProblemDetails>(request, true);

        // assert
        message.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        problemDetails.Errors.ShouldContain(error => error.Name == "speciesId" && error.Reason == "Invalid species ID");
    }

    [Fact]
    public async Task AddPost_WithAnonymousUser_ShouldReturnUnauthorized()
    {
        // arrange
        var httpClient = fixture.Client;
        var request = CreateRequest();

        // act
        var message = await httpClient.POSTAsync<AddPostEndpoint, AddPostRequest>(request, true);

        // assert
        message.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    private static AddPostRequest CreateRequest(
        string title = "Test Post",
        string description = "This is a test post",
        int speciesId = 1) => new()
    {
        Title = title,
        Description = description,
        Image = CreateTestImage(),
        SpeciesId = speciesId,
        Tags = ["cute", "funny", "dog"]
    };

    private static FormFile CreateTestImage()
    {
        var stream = new MemoryStream();
        var bytes = new byte[1024];
        Random.Shared.NextBytes(bytes);
        stream.Write(bytes, 0, bytes.Length);
        stream.Position = 0;

        return new FormFile(stream, 0, stream.Length, "image", "image.jpeg")
        {
            Headers = new HeaderDictionary(),
            ContentType = "image/jpeg"
        };
    }
}
