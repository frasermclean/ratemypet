using Microsoft.Extensions.Time.Testing;

namespace RateMyPet.Core.Tests;

public class PostTests
{
    [Fact]
    public void NewPost_WithValidInput_ShouldCreatePostWithExpectedProperties()
    {
        // arrange
        const string title = "Test Post";
        var userId = Guid.NewGuid();
        const int speciesId = 1;

        // act
        var post = new Post(title, userId, speciesId);

        // assert
        post.Id.ShouldNotBe(Guid.Empty);
        post.Slug.ShouldStartWith("test-post-");
        post.UserId.ShouldBe(userId);
        post.Title.ShouldBe(title);
        post.Activities.ShouldHaveSingleItem().Category.ShouldBe(UserActivityCategory.AddPost);
    }

    [Fact]
    public void NewPost_WithLongTitle_ShouldThrowArgumentException()
    {
        // arrange
        var longTitle = new string('a', Post.TitleMaxLength + 1);
        var userId = Guid.NewGuid();
        const int speciesId = 1;

        // act & assert
        Should.Throw<ArgumentException>(() => new Post(longTitle, userId, speciesId));
    }

    [Fact]
    public void NewPost_WithEmptyTitle_ShouldThrowArgumentException()
    {
        // arrange
        var userId = Guid.NewGuid();
        const int speciesId = 1;

        // act & assert
        Should.Throw<ArgumentException>(() => new Post(string.Empty, userId, speciesId));
    }

    [Theory]
    [InlineData("My first post.", 1234, "my-first-post-1234")]
    [InlineData("  COOL  STORY!!  ", 1735603200, "cool-story-1735603200")]
    [InlineData("What do you think?!", 54321, "what-do-you-think-54321")]
    public void CreateSlug_WithValidInput_ShouldReturnExpectedResult(string title, long timeStamp, string expectedSlug)
    {
        // arrange
        var timeProvider = new FakeTimeProvider(DateTimeOffset.FromUnixTimeSeconds(timeStamp));

        // act
        var slug = Post.CreateSlug(title, timeProvider);

        // assert
        slug.ShouldBe(expectedSlug);
    }
}
