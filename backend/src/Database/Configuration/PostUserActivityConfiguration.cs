using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RateMyPet.Core;
using RateMyPet.Database.Converters;

namespace RateMyPet.Database.Configuration;

public class PostUserActivityConfiguration : IEntityTypeConfiguration<PostUserActivity>
{
    public void Configure(EntityTypeBuilder<PostUserActivity> builder)
    {
        builder.Property(activity => activity.Reaction)
            .HasConversion<ReactionConverter>()
            .HasColumnType("char(1)");
    }
}
