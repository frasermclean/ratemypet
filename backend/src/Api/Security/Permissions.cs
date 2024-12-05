namespace RateMyPet.Api.Security;

public static class Permissions
{
    public static class Posts
    {
        public const string Add = "posts.add";
        public const string DeleteOwned = "posts.delete.owned";
        public const string DeleteAny = "posts.delete.any";
    }

    public static class Comments
    {
        public const string Add = "comments.add";
    }
}
