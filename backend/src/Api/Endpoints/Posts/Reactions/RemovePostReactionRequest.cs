using System.Security.Claims;
using FastEndpoints;

namespace RateMyPet.Api.Endpoints.Posts.Reactions;

public class RemovePostReactionRequest
{
    public Guid PostId { get; init; }
    [FromClaim(ClaimTypes.NameIdentifier)] public Guid UserId { get; init; }
}
