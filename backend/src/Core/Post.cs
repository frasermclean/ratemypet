namespace RateMyPet.Core;

public class Post
{
    public Guid Id { get; init; }
    public required string Title { get; set; }
    public required string Caption { get; set; }
}