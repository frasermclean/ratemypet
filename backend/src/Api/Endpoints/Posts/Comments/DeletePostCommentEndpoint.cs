using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using RateMyPet.Persistence.Services;

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

        var comment = await dbContext.Posts
            .Where(post => post.Id == postId)
            .SelectMany(post => post.Comments)
            .Where(comment => comment.Id == commentId)
            .FirstOrDefaultAsync(cancellationToken);

        if (comment is null)
        {
            Logger.LogError("Comment {CommentId} not found", commentId);
            return TypedResults.NotFound();
        }

        dbContext.PostComments.Remove(comment);
        await dbContext.SaveChangesAsync(cancellationToken);

        Logger.LogInformation("Deleted comment {CommentId} from post {PostId}", commentId, postId);

        return TypedResults.NoContent();
    }
}
