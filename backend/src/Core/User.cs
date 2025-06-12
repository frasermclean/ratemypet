using Microsoft.AspNetCore.Identity;

namespace RateMyPet.Core;

public class User : IdentityUser<Guid>
{
    public const int UserNameMaxLength = 32;

    public DateTime? LastActivity { get; set; }
    public ICollection<Role> Roles { get; } = [];
    public ICollection<Post> Posts { get; } = [];
    public ICollection<UserActivity> Activities { get; } = [];
    public ulong RowVersion { get; init; }
}
