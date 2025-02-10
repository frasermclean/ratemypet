using System.Security.Claims;
using FastEndpoints;

namespace RateMyPet.Api.Endpoints.Posts;

public class AddPostRequest
{
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required IFormFile Image { get; init; }
    public int SpeciesId { get; init; }
    public List<string> Tags { get; init; } = [];
    [FromClaim(ClaimTypes.NameIdentifier)] public Guid UserId { get; init; }
}
