using System.Security.Claims;
using FastEndpoints;
using RateMyPet.Core;

namespace RateMyPet.Api.Endpoints.Posts.Reactions;

public class AddPostReactionRequest
{
    public Guid PostId { get; init; }
    public Reaction Reaction { get; init; }
    [FromClaim(ClaimTypes.NameIdentifier)] public Guid UserId { get; init; }
}
