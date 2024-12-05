namespace RateMyPet.Api.Security;

public static class Permissions
{
    public static class Posts
    {
        public const string Add = "Posts.Add";
        public const string EditOwned = "Posts.Edit.Owned";
        public const string DeleteOwned = "Posts.Delete.Owned";
    }
}
