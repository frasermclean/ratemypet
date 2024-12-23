using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RateMyPet.Core;

namespace RateMyPet.Persistence.Configuration;

public class PostConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.Property(post => post.Id)
            .HasDefaultValueSql("newid()");

        builder.Property(post => post.Title)
            .HasMaxLength(Post.TitleMaxLength);

        builder.Property(post => post.Description)
            .HasMaxLength(Post.DescriptionMaxLength);

        builder.Property(post => post.CreatedAtUtc)
            .HasPrecision(2)
            .HasDefaultValueSql("getutcdate()");

        builder.Property(post => post.UpdatedAtUtc)
            .HasPrecision(2);

        builder.Property(post => post.RowVersion)
            .IsRowVersion()
            .HasConversion<byte[]>();
    }
}
