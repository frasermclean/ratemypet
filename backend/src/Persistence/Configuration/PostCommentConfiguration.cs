using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RateMyPet.Core;

namespace RateMyPet.Persistence.Configuration;

public class PostCommentConfiguration : IEntityTypeConfiguration<PostComment>
{
    public void Configure(EntityTypeBuilder<PostComment> builder)
    {
        builder.Property(comment => comment.Id)
            .HasDefaultValueSql("newid()");

        builder.Property(comment => comment.Content)
            .HasMaxLength(PostComment.ContentMaxLength);

        builder.Property(comment => comment.CreatedAtUtc)
            .HasPrecision(2)
            .HasDefaultValueSql("getutcdate()");

        builder.Property(comment => comment.UpdatedAtUtc)
            .HasPrecision(2);

        builder.Property(comment => comment.RowVersion)
            .IsRowVersion()
            .HasConversion<byte[]>();
    }
}
