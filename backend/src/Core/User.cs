using Microsoft.AspNetCore.Identity;

namespace RateMyPet.Core;

public class User : IdentityUser<Guid>
{
    public ICollection<Post> Posts { get; } = [];
    public ICollection<PostReaction> PostReactions { get; } = [];
    public DateTime? LastSeen { get; set; }
    public ulong RowVersion { get; init; }
}
