using EntityFramework.Exceptions.Common;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using RateMyPet.Api.Extensions;
using RateMyPet.Core;
using RateMyPet.Database;

namespace RateMyPet.Api.Endpoints.Posts.Comments;

public class AddPostCommentEndpoint(ApplicationDbContext dbContext)
    : Endpoint<AddPostCommentRequest, Ok<PostCommentResponse>>
{
    public override void Configure()
    {
        Post("posts/{postId:guid}/comments");
        Roles(Role.Contributor);
    }

    public override async Task<Ok<PostCommentResponse>> ExecuteAsync(AddPostCommentRequest request,
        CancellationToken cancellationToken)
    {
        var comment = MapToComment(request);

        dbContext.Add(comment);
        dbContext.Add(PostUserActivity.AddComment(request.UserId, request.PostId, comment));

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (ReferenceConstraintException exception) when (exception.ConstraintProperties.Contains("PostId"))
        {
            Logger.LogError(exception, "Could not add comment to post with ID {PostId}", request.PostId);
            ThrowError(r => r.PostId, "Invalid post ID");
        }

        Logger.LogInformation("Added comment with ID {CommentId} to post with ID {PostId} by user with ID {UserId}",
            comment.Id, request.PostId, request.UserId);

        return TypedResults.Ok(new PostCommentResponse
        {
            Id = comment.Id,
            Content = comment.Content,
            AuthorUserName = User.GetUserName()!,
            CreatedAtUtc = comment.CreatedAtUtc
        });
    }

    private static PostComment MapToComment(AddPostCommentRequest request) => new()
    {
        PostId = request.PostId,
        UserId = request.UserId,
        Content = request.Content,
        ParentId = request.ParentId
    };
}
