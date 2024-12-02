using System.Text.Json.Serialization;

namespace RateMyPet.Api.Endpoints.Posts;

public class PostCommentResponse
{
    public Guid Id { get; init; }
    public required string Content { get; init; }
    public required string AuthorUserName { get; init; }
    public DateTime CreatedAtUtc { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime? UpdatedAtUtc { get; set; }

    [JsonIgnore] public Guid? ParentId { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IEnumerable<PostCommentResponse>? Replies { get; set; }
}
