using System.Security.Claims;
using FastEndpoints;

namespace RateMyPet.Api.Endpoints.Posts;

public class DeletePostRequest : IModifyPostRequest
{
    public Guid PostId { get; init; }
    public bool? ShouldHardDelete { get; init; }
    [FromClaim(ClaimTypes.NameIdentifier)] public Guid UserId { get; init; }
}
