namespace RateMyPet.Core;

public class PostUserActivity : UserActivity
{
    public Guid PostId { get; init; }
    public Post? Post { get; init; }

    public static PostUserActivity AddPost(Guid userId, Guid postId) => new()
    {
        UserId = userId,
        PostId = postId,
        Code = "ADDP"
    };

    public static PostUserActivity UpdatePost(Guid userId, Guid postId) => new()
    {
        UserId = userId,
        PostId = postId,
        Code = "UPDP"
    };
}
