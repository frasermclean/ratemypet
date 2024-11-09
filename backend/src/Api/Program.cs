using RateMyPet.Core;

namespace RateMyPet.Api;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddAuthorization();
        builder.Services.AddEndpointsApiExplorer();

        var app = builder.Build();

        app.MapGet("/api/posts", () => SamplePosts);

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.Run();
    }

    private static readonly IEnumerable<Post> SamplePosts = new List<Post>
    {
        new() { Id = Guid.NewGuid(), Title = "First Post", Caption = "This is the first post" },
        new() { Id = Guid.NewGuid(), Title = "Second Post", Caption = "This is the second post" },
        new() { Id = Guid.NewGuid(), Title = "Third Post", Caption = "This is the third post" }
    };
}