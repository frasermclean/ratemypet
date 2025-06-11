using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RateMyPet.Core;

namespace RateMyPet.Database.Tests;

[Trait("Category", "Integration")]
public class UsersTests(DatabaseFixture fixture)
{
    [Fact]
    public async Task AddUser_WithValidUser_ShouldExecuteInterceptor()
    {
        // arrange
        await using var scope = fixture.ServiceProvider.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var user = CreateUser();

        // act
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
        var addedUser = await dbContext.Users.Where(u => u.Id == user.Id)
            .Include(u => u.Activities)
            .FirstOrDefaultAsync(TestContext.Current.CancellationToken);

        // assert
        addedUser.ShouldNotBeNull();
        addedUser.Activities.First()
            .ShouldSatisfyAllConditions(activity => activity.Activity.ShouldBe(Activity.Register));
    }

    private static User CreateUser(string userName = "bob.smith", string email = "bob.smith@example.com") => new()
    {
        Id = Guid.NewGuid(),
        UserName = userName,
        NormalizedUserName = userName.ToUpperInvariant(),
        Email = email,
        NormalizedEmail = email.ToUpperInvariant(),
    };
}
