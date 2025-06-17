using System.Text.Json.Serialization;

namespace RateMyPet.Api.Endpoints.Posts;

public class PostCommentResponse
{
    public Guid Id { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Content { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool IsDeleted { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? AuthorUserName { get; init; }

    public DateTime CreatedAtUtc { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime? UpdatedAtUtc { get; set; }

    [JsonIgnore] public Guid? ParentId { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IEnumerable<PostCommentResponse>? Replies { get; set; }
}
