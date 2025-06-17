using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using RateMyPet.Core;
using RateMyPet.Database;

namespace RateMyPet.Api.Endpoints.Posts.Comments;

public class DeletePostCommentEndpoint(ApplicationDbContext dbContext)
    : Endpoint<DeletePostCommentRequest, Results<NoContent, NotFound, ForbidHttpResult>>
{
    public override void Configure()
    {
        Delete("posts/{postId:guid}/comments/{commentId:guid}");
        Roles(Role.Contributor, Role.Administrator);
    }

    public override async Task<Results<NoContent, NotFound, ForbidHttpResult>> ExecuteAsync(
        DeletePostCommentRequest request,
        CancellationToken cancellationToken)
    {
        var comment = await dbContext.PostComments.FirstOrDefaultAsync(
            c => c.Id == request.CommentId && c.PostId == request.PostId, cancellationToken);

        if (comment is null)
        {
            Logger.LogError("Comment with {CommentId} not found", request.CommentId);
            return TypedResults.NotFound();
        }

        if (comment.UserId != request.UserId || !User.IsInRole(Role.Administrator))
        {
            Logger.LogError("User {UserId} attempted to delete comment {CommentId} without permission",
                request.UserId, request.CommentId);

            return TypedResults.Forbid();
        }

        comment.DeletedAtUtc = DateTime.UtcNow;
        dbContext.Add(PostUserActivity.DeleteComment(request.UserId, request.PostId, comment));

        await dbContext.SaveChangesAsync(cancellationToken);

        Logger.LogInformation("Deleted comment {CommentId} from post {PostId}", request.CommentId, request.PostId);
        return TypedResults.NoContent();
    }
}
