namespace RateMyPet.Api.Security;

public static class Roles
{
    public const string User = "User";

    public static IReadOnlyDictionary<string, string[]> PermissionsMap { get; } = new Dictionary<string, string[]>
    {
        { User, [Permissions.Posts.Add, Permissions.Posts.EditOwned, Permissions.Posts.DeleteOwned] }
    };
}
