using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using RateMyPet.Database;

namespace RateMyPet.Api.Endpoints.Posts.Comments;

public class DeletePostCommentEndpoint(ApplicationDbContext dbContext)
    : EndpointWithoutRequest<Results<NoContent, NotFound>>
{
    public override void Configure()
    {
        Delete("posts/{postId:guid}/comments/{commentId:guid}");
    }

    public override async Task<Results<NoContent, NotFound>> ExecuteAsync(CancellationToken cancellationToken)
    {
        var postId = Route<Guid>("postId");
        var commentId = Route<Guid>("commentId");

        var deletedCount = await dbContext.PostComments
            .Where(comment => comment.Id == commentId && comment.Post.Id == postId)
            .ExecuteDeleteAsync(cancellationToken);

        if (deletedCount == 0)
        {
            Logger.LogError("Comment {CommentId} not found", commentId);
            return TypedResults.NotFound();
        }

        Logger.LogInformation("Deleted comment {CommentId} from post {PostId}", commentId, postId);
        return TypedResults.NoContent();
    }
}
