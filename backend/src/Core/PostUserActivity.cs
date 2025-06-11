namespace RateMyPet.Core;

public class PostUserActivity : UserActivity
{
    public Post? Post { get; init; }

    public static PostUserActivity AddPost(User user, Post post) => new()
    {
        User = user,
        Post = post,
        Type = ActivityType.AddPost
    };
}
