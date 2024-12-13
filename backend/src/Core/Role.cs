using Microsoft.AspNetCore.Identity;

namespace RateMyPet.Core;

public class Role : IdentityRole<Guid>
{
    public ICollection<User> Users { get; } = [];
}
