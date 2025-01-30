namespace RateMyPet.Api.Endpoints.Posts.Images;

public class UpdatePostImageRequest
{
    public Guid PostId { get; init; }
    public required string ImageId { get; init; }
}
