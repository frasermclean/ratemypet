using RateMyPet.Core;

namespace RateMyPet.Initializer;

public static class SeedData
{
    public static readonly Dictionary<string, User> Users = new()
    {
        {
            Role.Administrator, new User
            {
                Id = Guid.Parse("fb8ad061-3a62-45f9-2202-08dd15f6fc85"),
                UserName = "frasermclean",
                NormalizedUserName = "frasermclean",
                Email = "dev@frasermclean.com",
                NormalizedEmail = "DEV@FRASERMCLEAN.COM",
                EmailConfirmed = true,
                PasswordHash = "AQAAAAIAAYagAAAAEBoXY3pqPk5/R8T+lWoMQvgHwVaoG3hT+R8xD1iwrLk8S6Xy4F1vJbFEx6XQLShF5w==",
                SecurityStamp = "SECHFYM7WURIXXX2FGC33PN3RWDOU4YD",
                ConcurrencyStamp = "d9bed53f-0212-4b82-8214-3cf7c32097b7"
            }
        },
        {
            Role.Contributor, new User
            {
                Id = Guid.Parse("bd9a2e18-d51a-4938-b864-584488f68456"),
                UserName = "bob.smith",
                NormalizedUserName = "BOB.SMITH",
                Email = "bob.smith@example.com",
                NormalizedEmail = "BOB.SMITH@EXAMPLE.COM",
                EmailConfirmed = true,
                PasswordHash = "AQAAAAIAAYagAAAAEFySNGZf8dB9Db+pZd4C6lvW3br+dntvxGPZlyGi9TTeipfgjv0mTIpooHgxrT6qkw==",
                SecurityStamp = "RHRRJ2ARLCN4AIEZL6XOKWRI2PCMCR7H",
                ConcurrencyStamp = "1a54bd41-ef74-482b-9322-4971eecee475"
            }
        }
    };

    public static readonly List<Post> Posts =
    [
        new("Alberto on the lawn", Users[Role.Administrator].Id, 1) // index 0 - this post will be read in tests
        {
            Description = "What a beautiful boy",
            Image = new PostImage
            {
                AssetId = "dfa21c4a5098ff546534df05982ccb09",
                PublicId = "samples/lab_puppy_1",
                Width = 1024,
                Height = 1024,
                Size = 97438
            },
            Tags = ["dog", "puppy", "cute"],
            Status = PostStatus.Approved
        },
        new("Soft kitty warm kitty", Users[Role.Contributor].Id, 2) // index 1 - this post will be deleted in tests
        {
            Description = "Little ball of fur",
            Image = new PostImage
            {
                AssetId = "957fe2c55eb1fc33951e442310217fc2",
                PublicId = "samples/ginger_sleeping",
                Width = 1024,
                Height = 1024,
                Size = 139418
            },
            Tags = ["cat", "sleeping", "fur", "ginger"],
            Status = PostStatus.Approved
        },
        new("Barry loves leaves", Users[Role.Contributor].Id, 1) // index 2 - this post will be updated in tests
        {
            Description = "He just loves finding these big piles of leaves and jumping right in them!",
            Image = new PostImage
            {
                AssetId = "4b1c80279fcf6865173b9a69a67e1902",
                PublicId = "samples/barry_leaves",
                Width = 1024,
                Height = 1024,
                Size = 223358
            },
            Tags = ["dog", "outdoor", "autumn", "leaves"],
            Status = PostStatus.Approved
        },
    ];
}
