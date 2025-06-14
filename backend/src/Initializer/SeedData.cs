using RateMyPet.Core;

namespace RateMyPet.Initializer;

public static class SeedData
{
    public static readonly List<Post> Posts =
    [
        new()
        {
            Id = Guid.NewGuid(),
            Slug = "first-post",
            Title = "First Post",
            Description = "This is the first post in the RateMyPet application.",
            UserId = Guid.Parse("fb8ad061-3a62-45f9-2202-08dd15f6fc85"),
            SpeciesId = 1,
            Status = PostStatus.Approved
        }
    ];

}
