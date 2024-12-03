namespace RateMyPet.Api.Endpoints.Posts;

public interface IModifyPostRequest
{
    Guid PostId { get; }
    Guid UserId { get; }
}
