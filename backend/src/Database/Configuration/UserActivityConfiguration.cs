using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RateMyPet.Core;

namespace RateMyPet.Database.Configuration;

public class UserActivityConfiguration : IEntityTypeConfiguration<UserActivity>
{
    public void Configure(EntityTypeBuilder<UserActivity> builder)
    {
        builder.ToTable("UserActivities");

        builder.HasDiscriminator<string>("Discriminator")
            .HasValue<UserActivity>("Base")
            .HasValue<PostUserActivity>("Post");

        builder.Property(activity => activity.Timestamp)
            .HasColumnName("TimestampUtc")
            .HasPrecision(2)
            .HasDefaultValueSql("getutcdate()");
    }
}
