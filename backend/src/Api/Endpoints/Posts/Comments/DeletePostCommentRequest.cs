using System.Security.Claims;
using FastEndpoints;

namespace RateMyPet.Api.Endpoints.Posts.Comments;

public class DeletePostCommentRequest
{
    public Guid PostId { get; init; }
    public Guid CommentId { get; init; }
    [FromClaim(ClaimTypes.NameIdentifier)] public Guid UserId { get; init; }
}
