namespace RateMyPet.Persistence.Models;

public class Species
{
    public const int NameMaxLength = 20;

    public int Id { get; init; }
    public required string Name { get; set; }
    public ulong RowVersion { get; init; }
    public ICollection<Post> Posts { get; } = [];
}
