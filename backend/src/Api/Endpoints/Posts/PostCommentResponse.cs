using System.Text.Json.Serialization;

namespace RateMyPet.Api.Endpoints.Posts;

public class PostCommentResponse
{
    public Guid Id { get; init; }
    public required string Content { get; init; }
    public required string UserName { get; init; }

    [JsonIgnore] public Guid? ParentId { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IEnumerable<PostCommentResponse>? Replies { get; set; }
}
