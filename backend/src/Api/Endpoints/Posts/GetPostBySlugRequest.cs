using System.Security.Claims;
using FastEndpoints;

namespace RateMyPet.Api.Endpoints.Posts;

public class GetPostBySlugRequest
{
    public required string PostSlug { get; init; }
}
