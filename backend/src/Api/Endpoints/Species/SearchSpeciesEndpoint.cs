using FastEndpoints;
using Gridify;
using Gridify.EntityFramework;
using RateMyPet.Persistence.Services;

namespace RateMyPet.Api.Endpoints.Species;

public class SearchSpeciesEndpoint(ApplicationDbContext dbContext) : Endpoint<GridifyQuery, Paging<SearchSpeciesMatch>>
{
    public override void Configure()
    {
        Get("species");
    }

    public override async Task<Paging<SearchSpeciesMatch>> ExecuteAsync(GridifyQuery query,
        CancellationToken cancellationToken)
    {
        var paging = await dbContext.Species
            .Select(species => new SearchSpeciesMatch
            {
                Id = species.Id,
                Name = species.Name,
                PostCount = species.Posts.Count
            })
            .GridifyAsync(query, cancellationToken);

        return paging;
    }
}
