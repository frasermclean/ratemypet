using Microsoft.AspNetCore.Identity;

namespace RateMyPet.Core;

public class User : IdentityUser<Guid>
{
    public ICollection<Post> Posts { get; } = [];
    public ICollection<PostReaction> PostReactions { get; } = [];
    public ulong RowVersion { get; init; }
}
