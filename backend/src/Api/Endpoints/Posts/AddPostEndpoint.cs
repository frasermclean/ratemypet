using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using RateMyPet.Api.Extensions;
using RateMyPet.Core;
using RateMyPet.Core.Abstractions;
using RateMyPet.Core.Messages;
using RateMyPet.Database;

namespace RateMyPet.Api.Endpoints.Posts;

public class AddPostEndpoint(
    ApplicationDbContext dbContext,
    IImageHostingService imageHostingService,
    IMessagePublisher messagePublisher)
    : Endpoint<AddPostRequest, Created<PostResponse>, PostResponseMapper>
{
    public override void Configure()
    {
        Post("posts");
        Roles(Role.Contributor);
        AllowFormData();
        AllowFileUploads();
    }

    public override async Task<Created<PostResponse>> ExecuteAsync(AddPostRequest request,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId() ?? throw new InvalidOperationException("User ID is not available");

        // create new post
        var post = new Post
        {
            Slug = Core.Post.CreateSlug(request.Title),
            Title = request.Title,
            Description = request.Description,
            UserId = userId,
            SpeciesId = request.SpeciesId,
            Tags = request.Tags.Distinct().ToList(),
        };

        // upload image to cloudinary
        var imageUploadResult = await imageHostingService.UploadAsync(request.Image.FileName,
            request.Image.OpenReadStream(), post, cancellationToken);

        if (imageUploadResult.IsFailed)
        {
            ThrowError(r => r.Image, "Error processing image upload");
        }

        post.Image = imageUploadResult.Value;

        // save the post entity
        dbContext.Posts.Add(post);
        dbContext.UserActivities.Add(PostUserActivity.AddPost(userId, post.Id));
        await dbContext.SaveChangesAsync(cancellationToken);

        Logger.LogInformation("Post with ID {PostId} was added successfully", post.Id);

        // publish message
        await messagePublisher.PublishAsync(new PostAddedMessage(post.Id), cancellationToken);

        var response = Map.FromEntity(post);
        return TypedResults.Created($"/posts/{response.Id}", response);
    }
}
