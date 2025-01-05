using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RateMyPet.Core;

namespace RateMyPet.Infrastructure.Configuration;

public class SpeciesConfiguration : IEntityTypeConfiguration<Species>
{
    public void Configure(EntityTypeBuilder<Species> builder)
    {
        builder.Property(species => species.Name)
            .HasMaxLength(Species.NameMaxLength);

        builder.Property(species => species.RowVersion)
            .IsRowVersion()
            .HasConversion<byte[]>();

        builder.HasData(
            new Species
            {
                Id = 1,
                Name = "Dog"
            }, new Species
            {
                Id = 2,
                Name = "Cat"
            },
            new Species
            {
                Id = 3,
                Name = "Bird"
            }
        );
    }
}
