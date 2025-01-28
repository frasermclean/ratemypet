namespace RateMyPet.Api.Endpoints.Posts;

public class GetPostRequest
{
    public Guid? PostId { get; init; }
    public string? PostSlug { get; init; }
}
