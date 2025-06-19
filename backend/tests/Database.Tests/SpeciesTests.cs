using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace RateMyPet.Database.Tests;

[Trait("Category", "Integration")]
public class SpeciesTests(DatabaseFixture fixture)
{
    [Fact]
    public async Task GetAllSpecies_ShouldNotBeEmpty()
    {
        // arrange
        await using var scope = fixture.ServiceProvider.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // act
        var speciesList = await dbContext.Species.ToListAsync(TestContext.Current.CancellationToken);

        // Assert
        speciesList.ShouldNotBeEmpty();
    }
}
