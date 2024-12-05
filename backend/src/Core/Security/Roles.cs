namespace RateMyPet.Core.Security;

public static class Roles
{
    public static readonly RoleDefinition User = new()
    {
        Id = new Guid("57c523c9-0957-4834-8fce-ff37fa861c36"),
        Name = "User"
    };

    public static readonly RoleDefinition Administrator = new()
    {
        Id = new Guid("8e71eb35-2194-495b-b0e8-8690ebe7f918"),
        Name = "Administrator"
    };

    public static IReadOnlyDictionary<RoleDefinition, string[]> PermissionsMap { get; } =
        new Dictionary<RoleDefinition, string[]>
        {
            { User, [Permissions.Posts.Add, Permissions.Posts.EditOwned, Permissions.Posts.DeleteOwned] }
        };
}
