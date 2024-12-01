using System.Security.Claims;
using FastEndpoints;

namespace RateMyPet.Api.Endpoints.Posts.Comments;

public class AddPostCommentRequest
{
    public Guid PostId { get; init; }
    public Guid? ParentId { get; init; }
    public required string Content { get; init; }
    [FromClaim(ClaimTypes.NameIdentifier)] public Guid UserId { get; init; }
}
