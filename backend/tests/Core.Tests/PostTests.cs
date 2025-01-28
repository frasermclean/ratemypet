using Microsoft.Extensions.Time.Testing;

namespace RateMyPet.Core;

public class PostTests
{
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
