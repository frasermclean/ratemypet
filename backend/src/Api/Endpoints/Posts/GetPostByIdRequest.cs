using System.Security.Claims;
using FastEndpoints;

namespace RateMyPet.Api.Endpoints.Posts;

public class GetPostByIdRequest
{
    public Guid PostId { get; init; }
}
