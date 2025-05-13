using Delta;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using RateMyPet.Database;

namespace RateMyPet.Api.Endpoints.Species;

public class GetAllSpeciesEndpoint(ApplicationDbContext dbContext)
    : EndpointWithoutRequest<IEnumerable<SpeciesResponse>>
{
    public override void Configure()
    {
        Get("species");
        Options(builder => builder.UseDelta<ApplicationDbContext>());
        AllowAnonymous();
    }

    public override async Task<IEnumerable<SpeciesResponse>> ExecuteAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Species
            .OrderBy(species => species.Name)
            .Select(species => new SpeciesResponse
            {
                Id = species.Id,
                Name = species.Name,
                PluralName = species.PluralName,
                PostCount = species.Posts.Count
            })
            .ToListAsync(cancellationToken);
    }
}
