﻿using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using RateMyPet.Api.Services;
using RateMyPet.Core;
using RateMyPet.Persistence;
using RateMyPet.Persistence.Services;
using SpeciesModel = RateMyPet.Core.Species;
using PostsPermissions = RateMyPet.Core.Security.Permissions.Posts;

namespace RateMyPet.Api.Endpoints.Posts;

public class AddPostEndpoint(
    ApplicationDbContext dbContext,
    ImageProcessor imageProcessor,
    [FromKeyedServices(BlobContainerNames.OriginalImages)]
    IBlobContainerManager blobContainerManager)
    : Endpoint<AddPostRequest, Results<Created, ErrorResponse>>
{
    public override void Configure()
    {
        Post("posts");
        Permissions(PostsPermissions.Add);
        AllowFormData();
        AllowFileUploads();
    }

    public override async Task<Results<Created, ErrorResponse>> ExecuteAsync(AddPostRequest request,
        CancellationToken cancellationToken)
    {
        var species = await dbContext.Species.FirstOrDefaultAsync(s => s.Id == request.SpeciesId, cancellationToken);
        if (species is null)
        {
            AddError(r => r.SpeciesId, "Invalid species ID");
            return new ErrorResponse(ValidationFailures);
        }

        var imageResult = await ProcessAndUploadImageAsync(request, cancellationToken);
        var post = await CreatePostEntityAsync(request, species, imageResult, cancellationToken);

        return TypedResults.Created($"/posts/{post.Id}");
    }

    private async Task<ProcessImageResult> ProcessAndUploadImageAsync(AddPostRequest request,
        CancellationToken cancellationToken)
    {
        var blobName = imageProcessor.GenerateBlobName();

        await using var readStream = request.Image.OpenReadStream();
        await using var writeStream = await blobContainerManager.OpenWriteStreamAsync(blobName,
            imageProcessor.ContentType, cancellationToken);

        var result = await imageProcessor.ProcessImageAsync(readStream, writeStream, blobName, cancellationToken);

        Logger.LogInformation("Image processed and uploaded to blob storage, blobName: {BlobName}", blobName);

        return result;
    }

    private async Task<Post> CreatePostEntityAsync(AddPostRequest request, SpeciesModel species,
        ProcessImageResult imageResult,
        CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.FirstAsync(user => user.Id == request.UserId, cancellationToken);
        var post = new Post
        {
            Title = request.Title,
            Description = request.Description,
            User = user,
            Species = species,
            Image = new PostImage
            {
                Width = imageResult.Width,
                Height = imageResult.Height,
                BlobName = imageResult.BlobName,
                ContentType = imageResult.ContentType
            }
        };

        user.Posts.Add(post);
        await dbContext.SaveChangesAsync(cancellationToken);

        Logger.LogInformation("Post with ID {PostId} was added successfully", post.Id);

        return post;
    }
}
