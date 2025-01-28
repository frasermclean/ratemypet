using System.Security.Claims;
using FastEndpoints;

namespace RateMyPet.Api.Endpoints.Posts;

public class UpdatePostRequest : IModifyPostRequest
{
    public Guid PostId { get; init; }
    public required string Description { get; init; }
    public int SpeciesId { get; init; }
    [FromClaim(ClaimTypes.NameIdentifier)] public Guid UserId { get; init; }
}
