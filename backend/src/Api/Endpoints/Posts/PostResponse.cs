namespace RateMyPet.Api.Endpoints.Posts;

public class PostResponse
{
    public const string SampleImageUrl =
        "https://adoptrixdemo.blob.core.windows.net/animal-images/alberto-2024-02-14/c70d9597-0d16-43eb-8600-288c03d18fed/full.webp";

    public required Guid Id { get; init; }
    public required string Title { get; init; }
    public required string Caption { get; init; }
    public required string ImageUrl { get; init; }
    public required string AuthorEmailHash { get; init; }
    public int LikeCount { get; init; }
    public int FunnyCount { get; init; }
    public int CrazyCount { get; init; }
    public int WowCount { get; init; }
    public int SadCount { get; init; }
}
