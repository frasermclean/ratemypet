using Microsoft.AspNetCore.Identity;

namespace RateMyPet.Persistence.Models;

public class User : IdentityUser<Guid>
{
    public List<Post> Posts { get; } = [];
    public List<PostReaction> PostReactions { get; } = [];
    public ulong RowVersion { get; init; }
}
