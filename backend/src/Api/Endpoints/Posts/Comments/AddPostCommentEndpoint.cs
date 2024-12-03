using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using RateMyPet.Persistence.Models;
using RateMyPet.Persistence.Services;

namespace RateMyPet.Api.Endpoints.Posts.Comments;

public class AddPostCommentEndpoint(ApplicationDbContext dbContext)
    : Endpoint<AddPostCommentRequest, Results<Ok<PostCommentResponse>, NotFound>>
{
    public override void Configure()
    {
        Post("posts/{postId:guid}/comments");
    }

    public override async Task<Results<Ok<PostCommentResponse>, NotFound>> ExecuteAsync(AddPostCommentRequest request,
        CancellationToken cancellationToken)
    {
        var post = await dbContext.Posts.FirstOrDefaultAsync(p => p.Id == request.PostId, cancellationToken);
        if (post is null)
        {
            Logger.LogWarning("Could not find post with ID {PostId} to add comment to", request.PostId);
            return TypedResults.NotFound();
        }

        var comment = new PostComment
        {
            Content = request.Content,
            User = await dbContext.Users.FirstAsync(user => user.Id == request.UserId, cancellationToken),
            Parent = request.ParentId is not null
                ? await dbContext.PostComments.FirstAsync(c => c.Id == request.ParentId, cancellationToken)
                : null
        };

        post.Comments.Add(comment);
        await dbContext.SaveChangesAsync(cancellationToken);

        Logger.LogInformation("Added comment with ID {CommentId} to post with ID {PostId} by user with ID {UserId}",
            comment.Id, post.Id, comment.User.Id);

        return TypedResults.Ok(new PostCommentResponse
        {
            Id = comment.Id,
            Content = comment.Content,
            AuthorUserName = comment.User.UserName!,
            CreatedAtUtc = comment.CreatedAtUtc
        });
    }
}
