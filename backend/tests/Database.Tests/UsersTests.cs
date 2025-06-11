using Bogus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RateMyPet.Core;

namespace RateMyPet.Database.Tests;

[Trait("Category", "Integration")]
public class UsersTests(DatabaseFixture fixture)
{
    [Fact]
    public void AddUser_WithValidUser_ShouldAddUserAndRegisterActivity()
    {
        // arrange
        using var scope = fixture.ServiceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var user = CreateFakeUser();

        // act
        dbContext.Users.Add(user);
        dbContext.SaveChanges();
        var addedUser = dbContext.Users.Where(u => u.Id == user.Id)
            .Include(u => u.Activities)
            .FirstOrDefault();

        // assert
        addedUser.ShouldNotBeNull();
        addedUser.Activities.ShouldHaveSingleItem()
            .ShouldSatisfyAllConditions(activity => activity.Type.ShouldBe(ActivityType.Register));
    }

    [Fact]
    public async Task UpdateUser_WithConfirmedEmail_ShouldAddConfirmEmailActivity()
    {
        // arrange
        await using var scope = fixture.ServiceProvider.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var user = CreateFakeUser();

        // act
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
        user.EmailConfirmed = true;
        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
        var updatedUser = await dbContext.Users.Where(u => u.Id == user.Id)
            .Include(u => u.Activities)
            .FirstOrDefaultAsync(TestContext.Current.CancellationToken);

        // assert
        updatedUser.ShouldNotBeNull();
        updatedUser.Activities.ShouldContain(activity => activity.Type == ActivityType.ConfirmEmail);
    }

    private static User CreateFakeUser(bool emailConfirmed = false)
    {
        var userFaker = new Faker<User>()
            .RuleFor(user => user.Id, faker => faker.Random.Guid())
            .RuleFor(user => user.UserName, faker => faker.Internet.UserName())
            .RuleFor(user => user.Email, faker => faker.Internet.Email())
            .RuleFor(user => user.EmailConfirmed, emailConfirmed)
            .FinishWith((_, user) =>
            {
                user.NormalizedUserName = user.UserName!.ToUpperInvariant();
                user.NormalizedEmail = user.Email!.ToUpperInvariant();
            });

        return userFaker.Generate();
    }
}
