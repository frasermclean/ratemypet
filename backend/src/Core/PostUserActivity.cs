namespace RateMyPet.Core;

public class PostUserActivity : UserActivity
{
    public Guid PostId { get; init; }
    public Post? Post { get; init; }

    public static PostUserActivity AddPost(Guid userId, Post post) => new()
    {
        UserId = userId,
        Post = post,
        Type = ActivityType.AddPost
    };
}
