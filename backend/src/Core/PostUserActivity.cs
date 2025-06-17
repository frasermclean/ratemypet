namespace RateMyPet.Core;

public class PostUserActivity : UserActivity
{
    public Guid PostId { get; init; }
    public Post Post { get; init; } = null!;
    public PostComment? Comment { get; init; }
    public Reaction? Reaction { get; init; }

    public static PostUserActivity AddPost(Guid userId, Post post) => new()
    {
        UserId = userId,
        Post = post,
        Category = UserActivityCategory.AddPost
    };

    public static PostUserActivity UpdatePost(Guid userId, Post post) => new()
    {
        UserId = userId,
        Post = post,
        Category = UserActivityCategory.UpdatePost
    };

    public static PostUserActivity DeletePost(Guid userId, Guid postId) => new()
    {
        UserId = userId,
        PostId = postId,
        Category = UserActivityCategory.DeletePost
    };

    public static PostUserActivity AddComment(Guid userId, Guid postId, PostComment comment) => new()
    {
        UserId = userId,
        PostId = postId,
        Comment = comment,
        Category = UserActivityCategory.AddComment
    };

    public static PostUserActivity DeleteComment(Guid userId, Guid postId, PostComment comment) => new()
    {
        UserId = userId,
        PostId = postId,
        Comment = comment,
        Category = UserActivityCategory.DeleteComment
    };

    public static PostUserActivity UpdateReaction(Guid userId, Guid postId, Reaction reaction) => new()
    {
        UserId = userId,
        PostId = postId,
        Reaction = reaction,
        Category = UserActivityCategory.UpdateReaction
    };
}
