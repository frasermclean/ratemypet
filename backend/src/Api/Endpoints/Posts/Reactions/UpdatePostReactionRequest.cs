using System.Security.Claims;
using FastEndpoints;
using RateMyPet.Persistence.Models;

namespace RateMyPet.Api.Endpoints.Posts.Reactions;

public class UpdatePostReactionRequest
{
    public Guid PostId { get; init; }
    public Reaction Reaction { get; init; }
    [FromClaim(ClaimTypes.NameIdentifier)] public Guid UserId { get; init; }
}
