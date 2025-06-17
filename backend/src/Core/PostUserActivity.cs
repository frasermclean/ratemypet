namespace RateMyPet.Core;

public class PostUserActivity : UserActivity
{
    public Guid PostId { get; init; }
    public Post? Post { get; init; }
    public PostComment? Comment { get; init; }
    public Guid? CommentId { get; init; }

    public static PostUserActivity AddPost(Guid userId, Post post) => new()
    {
        UserId = userId,
        Post = post,
        Code = "ADDP"
    };

    public static PostUserActivity UpdatePost(Guid userId, Post post) => new()
    {
        UserId = userId,
        Post = post,
        Code = "UPDP"
    };

    public static PostUserActivity DeletePost(Guid userId, Guid postId) => new()
    {
        UserId = userId,
        PostId = postId,
        Code = "DELP"
    };

    public static PostUserActivity AddComment(Guid userId, Guid postId, PostComment comment) => new()
    {
        UserId = userId,
        PostId = postId,
        Comment = comment,
        Code = "ADDC",
    };

    public static PostUserActivity DeleteComment(Guid userId, Guid postId, PostComment comment) => new()
    {
        UserId = userId,
        PostId = postId,
        Comment = comment,
        Code = "DELC"
    };
}
