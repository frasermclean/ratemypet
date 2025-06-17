using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using RateMyPet.Core;
using RateMyPet.Database;

namespace RateMyPet.Api.Endpoints.Posts.Comments;

public class AddPostCommentEndpoint(ApplicationDbContext dbContext)
    : Endpoint<AddPostCommentRequest, Results<Ok<PostCommentResponse>, NotFound>>
{
    public override void Configure()
    {
        Post("posts/{postId:guid}/comments");
        Roles(Role.Contributor);
    }

    public override async Task<Results<Ok<PostCommentResponse>, NotFound>> ExecuteAsync(AddPostCommentRequest request,
        CancellationToken cancellationToken)
    {
        var comment = MapToComment(request);

        dbContext.Add(comment);
        dbContext.Add(PostUserActivity.AddComment(request.UserId, request.PostId, comment));

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception exception)
        {
            Logger.LogError(exception, "Failed to add comment to post with ID {PostId} by user with ID {UserId}",
                request.PostId, request.UserId);

            throw;
        }

        Logger.LogInformation("Added comment with ID {CommentId} to post with ID {PostId} by user with ID {UserId}",
            comment.Id, request.PostId, request.UserId);

        return TypedResults.Ok(new PostCommentResponse
        {
            Id = comment.Id,
            Content = comment.Content,
            AuthorUserName = comment.User?.UserName ?? string.Empty,
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
