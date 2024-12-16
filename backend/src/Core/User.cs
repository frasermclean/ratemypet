using Microsoft.AspNetCore.Identity;

namespace RateMyPet.Core;

public class User : IdentityUser<Guid>
{
    public DateTime? LastSeen { get; set; }
    public ICollection<Role> Roles { get; } = [];
    public ulong RowVersion { get; init; }
}
